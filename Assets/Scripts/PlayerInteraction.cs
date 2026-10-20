using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [Header("Configuration")]
    public float interactionDistance = 6f;
    public KeyCode interactKey = KeyCode.E;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (mainCamera == null) return;

        // Origine du rayon (légèrement décalée vers l'avant pour éviter la hitbox du joueur)
        Vector3 rayOrigin = mainCamera.transform.position + mainCamera.transform.forward * 0.5f;
        Vector3 rayDirection = mainCamera.transform.forward;

        // Trace un rayon rouge très visible dans la vue Scene pendant 0.1s
        Debug.DrawRay(rayOrigin, rayDirection * interactionDistance, Color.red, 0.1f);

        // Physics.Raycast avec Ignore pour ignorer la Heating Zone (Triggers)
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, interactionDistance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
        {
            if (Input.GetKeyDown(interactKey))
            {
                Debug.Log("[Raycast] Touché : " + hit.collider.gameObject.name);

                ItemPickup item = hit.collider.GetComponent<ItemPickup>();
                if (item != null)
                {
                    item.Interact();
                }
            }
        }
    }
}