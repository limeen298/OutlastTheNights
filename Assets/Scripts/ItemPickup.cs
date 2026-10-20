using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Paramètres")]
    public ResourceType resourceType = ResourceType.Wood;
    public int amount = 1;

    public void Interact()
    {
        Debug.Log("[ItemPickup] Tentative de ramassage de : " + gameObject.name);

        Inventory playerInventory = FindFirstObjectByType<Inventory>();

        if (playerInventory != null)
        {
            playerInventory.AddResource(resourceType, amount);
            Debug.Log("[ItemPickup] Objet ajouté avec succès. Destruction du GameObject.");
            Destroy(gameObject);
        }
        else
        {
            Debug.LogError("[ItemPickup] ERREUR : Aucun composant Inventory trouvé sur la scène !");
        }
    }
}