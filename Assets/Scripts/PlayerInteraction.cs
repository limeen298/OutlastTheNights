using UnityEngine;

/// <summary>
/// Détecte l'objet visé par le joueur (Raycast depuis la caméra),
/// affiche l'indication "[E] Ramasser" et gère le ramassage.
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuration")]
    [Tooltip("Distance maximale pour interagir (en mètres).")]
    public float interactionDistance = 3f;
    public KeyCode interactKey = KeyCode.E;

    [Tooltip("Layers pris en compte par le Raycast.")]
    [SerializeField] private LayerMask interactionMask = Physics.DefaultRaycastLayers;

    [Header("Références")]
    [Tooltip("Inventaire du joueur (trouvé automatiquement si vide).")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private InteractionPromptUI promptUI;

    private Camera mainCamera;
    private ItemPickup currentTarget;

    private void Start()
    {
        mainCamera = Camera.main;

        if (inventory == null)
            inventory = FindFirstObjectByType<Inventory>();
    }

    private void Update()
    {
        if (mainCamera == null) return;

        currentTarget = FindTarget();
        UpdatePrompt();

        // Ramassage uniquement si un objet est visé et à portée
        if (currentTarget != null && Input.GetKeyDown(interactKey))
        {
            if (currentTarget.Interact(inventory))
            {
                currentTarget = null;
                promptUI?.Hide();
            }
        }
    }

    /// <summary>
    /// Lance un rayon depuis la caméra et retourne l'objet ramassable visé (ou null).
    /// </summary>
    private ItemPickup FindTarget()
    {
        Transform cam = mainCamera.transform;

        // Origine légèrement décalée vers l'avant pour éviter le collider du joueur
        Vector3 origin = cam.position + cam.forward * 0.5f;

        Debug.DrawRay(origin, cam.forward * interactionDistance, Color.red);

        // QueryTriggerInteraction.Ignore : on ignore les zones (ex : Heating Zone)
        if (Physics.Raycast(origin, cam.forward, out RaycastHit hit, interactionDistance,
                            interactionMask, QueryTriggerInteraction.Ignore))
        {
            // GetComponentInParent : fonctionne aussi si le collider est sur un enfant du modèle
            return hit.collider.GetComponentInParent<ItemPickup>();
        }

        return null;
    }

    /// <summary>
    /// Affiche ou cache l'indication selon l'objet visé.
    /// </summary>
    private void UpdatePrompt()
    {
        if (promptUI == null) return;

        if (currentTarget == null)
        {
            promptUI.Hide();
        }
        else if (currentTarget.CanBePickedUp(inventory))
        {
            promptUI.Show($"[{interactKey}] Ramasser");
        }
        else
        {
            promptUI.Show("Inventaire plein");
        }
    }
}