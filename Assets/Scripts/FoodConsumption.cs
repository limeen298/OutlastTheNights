
using UnityEngine;
using UnityEngine.UI;

public class FoodConsumption : MonoBehaviour
{
    [Header("Touches")]
    public KeyCode eatKey = KeyCode.F;

    [Header("Effets des aliments")]
    public float hungerRestored = 30f;
    public float poisonDamage = 20f;

    [Header("Contours des cases de l'inventaire")]
    public Outline[] slotOutlines = new Outline[3];

    private Inventory inventory;
    private PlayerStats playerStats;

    private int selectedSlot = 0;

    void Awake()
    {
        inventory = GetComponent<Inventory>();
        playerStats = GetComponent<PlayerStats>();
    }

    void Start()
    {
        UpdateSelectionVisual();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SelectSlot(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            SelectSlot(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            SelectSlot(2);

        if (Input.GetKeyDown(eatKey))
            EatSelectedFood();
    }

    void SelectSlot(int index)
    {
        if (inventory == null)
            return;

        if (index < 0 || index >= Inventory.SlotCount)
            return;

        selectedSlot = index;

        UpdateSelectionVisual();

        Inventory.Slot slot = inventory.GetSlot(selectedSlot);

        if (slot.IsEmpty)
            Debug.Log("Case " + (selectedSlot + 1) + " selectionnee : vide");
        else
            Debug.Log("Case " + (selectedSlot + 1) +
                      " selectionnee : " + slot.type);
    }

    void UpdateSelectionVisual()
    {
        for (int i = 0; i < slotOutlines.Length; i++)
        {
            if (slotOutlines[i] != null)
                slotOutlines[i].enabled = (i == selectedSlot);
        }
    }

    void EatSelectedFood()
    {
        if (inventory == null || playerStats == null)
        {
            Debug.LogError("Inventory ou PlayerStats manquant !");
            return;
        }

        Inventory.Slot slot = inventory.GetSlot(selectedSlot);

        if (slot.IsEmpty)
        {
            Debug.Log("La case selectionnee est vide !");
            return;
        }

        if (slot.type == ResourceType.Food)
        {
            if (playerStats.hunger >= 100f)
            {
                Debug.Log("La jauge de faim est deja pleine !");
                return;
            }

            if (inventory.TryRemoveResource(ResourceType.Food, 1))
            {
                playerStats.RestoreHunger(hungerRestored);
                Debug.Log("Viande mangee ! +30 points de faim.");
            }
        }
        else if (slot.type == ResourceType.PoisonousBerry)
        {
            if (inventory.TryRemoveResource(ResourceType.PoisonousBerry, 1))
            {
                playerStats.TakeDamage(poisonDamage);
                Debug.Log("Baie toxique mangee ! -20 points de sante.");
            }
        }
        else
        {
            Debug.Log("Cet objet n'est pas comestible !");
        }
    }
}

