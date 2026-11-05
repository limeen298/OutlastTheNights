using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Affiche les 3 cases de l'inventaire en bas de l'écran :
/// une icône par case + un texte "Ressource Quantité" (ex : "Wood 4").
/// Se met à jour uniquement quand l'inventaire change (pas de travail dans Update).
/// </summary>
public class InventoryUI : MonoBehaviour
{
    /// <summary>
    /// Apparence d'un type de ressource dans l'UI.
    /// </summary>
    [Serializable]
    private class ResourceVisual
    {
        public ResourceType type;
        [Tooltip("Icône de la ressource (optionnelle).")]
        public Sprite icon;
        [Tooltip("Couleur utilisée si aucune icône n'est fournie.")]
        public Color fallbackColor = Color.white;
    }

    [Header("Références")]
    [Tooltip("Inventaire du joueur (trouvé automatiquement si vide).")]
    [SerializeField] private Inventory inventory;

    [Tooltip("Une Image d'icône par case, dans l'ordre (3 éléments).")]
    [SerializeField] private Image[] slotIcons = new Image[Inventory.SlotCount];

    [Tooltip("Un texte TextMeshPro par case, dans l'ordre (3 éléments). Affiche 'Wood 4'.")]
    [FormerlySerializedAs("slotCounts")] // Conserve les références déjà assignées dans la scène
    [SerializeField] private TMP_Text[] slotLabels = new TMP_Text[Inventory.SlotCount];

    [Header("Apparence des ressources")]
    [SerializeField] private ResourceVisual[] visuals;

    private void Awake()
    {
        if (inventory == null)
            inventory = FindFirstObjectByType<Inventory>();
    }

    private void OnEnable()
    {
        if (inventory == null)
        {
            Debug.LogError("[InventoryUI] Aucun Inventory trouvé dans la scène !", this);
            return;
        }

        ValidateReferences();

        inventory.OnInventoryChanged += Refresh;
        Refresh(); // Affichage initial
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= Refresh;
    }

    /// <summary>
    /// Signale clairement dans la Console toute référence oubliée dans l'Inspector.
    /// </summary>
    private void ValidateReferences()
    {
        for (int i = 0; i < Inventory.SlotCount; i++)
        {
            if (i >= slotIcons.Length || slotIcons[i] == null)
                Debug.LogError($"[InventoryUI] 'Slot Icons' : élément {i} non assigné.", this);

            if (i >= slotLabels.Length || slotLabels[i] == null)
                Debug.LogError($"[InventoryUI] 'Slot Labels' : élément {i} non assigné (utiliser un TextMeshPro).", this);
        }
    }

    /// <summary>
    /// Met à jour l'affichage des 3 cases.
    /// </summary>
    private void Refresh()
    {
        int count = Mathf.Min(Inventory.SlotCount, slotIcons.Length, slotLabels.Length);

        for (int i = 0; i < count; i++)
        {
            Inventory.Slot slot = inventory.GetSlot(i);
            Image icon = slotIcons[i];
            TMP_Text label = slotLabels[i];

            if (slot.IsEmpty)
            {
                // Case vide : on cache l'icône et le texte
                if (icon != null) icon.enabled = false;
                if (label != null) label.text = string.Empty;
                continue;
            }

            if (icon != null)
            {
                ResourceVisual visual = GetVisual(slot.type);

                icon.enabled = true;
                icon.sprite = visual != null ? visual.icon : null;
                icon.color = (visual != null && visual.icon == null) ? visual.fallbackColor : Color.white;
            }

            // Texte sous la case : nom de la ressource + quantité (ex : "Wood 4")
            if (label != null)
                label.text = $"{slot.type} {slot.count}";
        }
    }

    /// <summary>
    /// Retrouve l'apparence associée à un type de ressource.
    /// </summary>
    private ResourceVisual GetVisual(ResourceType type)
    {
        if (visuals == null) return null;

        for (int i = 0; i < visuals.Length; i++)
        {
            if (visuals[i].type == type)
                return visuals[i];
        }

        return null;
    }
}