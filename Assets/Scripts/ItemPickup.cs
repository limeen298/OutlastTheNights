using UnityEngine;

/// <summary>
/// Objet ramassable (branche, pierre, corde...).
/// À placer sur la RACINE du prefab (l'objet parent), et non sur un enfant (ex : "Collider").
/// L'objet doit avoir un Collider NON trigger (peut être sur un enfant) pour être détecté par le Raycast.
/// </summary>
public class ItemPickup : MonoBehaviour
{
    [Header("Paramètres")]
    public ResourceType resourceType = ResourceType.Wood;
    [Min(1)] public int amount = 1;

    [Header("Avancé")]
    [Tooltip("Objet détruit au ramassage. Si vide, c'est le GameObject qui porte ce script.")]
    [SerializeField] private GameObject objectToDestroy;

    // Empêche un double ramassage : Destroy() n'est effectif qu'en fin de frame,
    // donc Interact() pourrait être appelé deux fois dans la même frame.
    private bool isCollected;

    /// <summary>
    /// Indique si l'inventaire a assez de place pour cet objet.
    /// </summary>
    public bool CanBePickedUp(Inventory inventory)
    {
        return !isCollected && inventory != null && inventory.HasRoomFor(resourceType, amount);
    }

    /// <summary>
    /// Ramasse l'objet : il est ajouté à l'inventaire puis disparaît de la scène.
    /// Retourne false si l'inventaire est plein (l'objet reste alors en place).
    /// </summary>
    public bool Interact(Inventory inventory)
    {
        // Déjà ramassé dans cette frame : on ignore
        if (isCollected) return false;

        if (inventory == null)
        {
            Debug.LogError("[ItemPickup] Aucun Inventory fourni !", this);
            return false;
        }

        if (!inventory.TryAddResource(resourceType, amount))
        {
            Debug.Log("[ItemPickup] Inventaire plein, ramassage impossible.");
            return false;
        }

        isCollected = true;

        // On détruit l'objet complet (mesh + collider), pas seulement l'enfant qui porte ce script
        GameObject target = objectToDestroy != null ? objectToDestroy : gameObject;
        Destroy(target);
        return true;
    }
}