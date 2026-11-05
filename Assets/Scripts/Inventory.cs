using System;
using UnityEngine;

/// <summary>
/// Types de ressources collectables dans le jeu.
/// </summary>
public enum ResourceType
{
    Wood,
    Stone,
    Rope
}

/// <summary>
/// Inventaire du joueur : 3 cases, chaque case contient un seul type de ressource (empilable).
/// À placer sur le GameObject du joueur.
/// </summary>
public class Inventory : MonoBehaviour
{
    /// <summary>
    /// Contenu d'une case de l'inventaire.
    /// </summary>
    [Serializable]
    public struct Slot
    {
        public ResourceType type;
        public int count;

        public bool IsEmpty => count <= 0;
    }

    /// <summary>Nombre de cases de l'inventaire.</summary>
    public const int SlotCount = 3;

    [Header("Configuration")]
    [Tooltip("Nombre maximum d'objets identiques dans une même case.")]
    [SerializeField, Min(1)] private int maxStackSize = 10;

    // Tableau fixe : pas d'allocation mémoire pendant le jeu
    private readonly Slot[] slots = new Slot[SlotCount];

    /// <summary>
    /// Déclenché à chaque modification de l'inventaire (l'UI s'y abonne).
    /// </summary>
    public event Action OnInventoryChanged;

    /// <summary>
    /// Retourne le contenu d'une case (0 à SlotCount - 1).
    /// </summary>
    public Slot GetSlot(int index) => slots[index];

    /// <summary>
    /// Place encore disponible pour un type de ressource donné
    /// (piles existantes non pleines + cases vides).
    /// </summary>
    public int GetFreeSpace(ResourceType type)
    {
        int free = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].IsEmpty)
                free += maxStackSize;
            else if (slots[i].type == type)
                free += maxStackSize - slots[i].count;
        }

        return free;
    }

    /// <summary>
    /// Indique si on peut ajouter 'amount' ressources de ce type.
    /// </summary>
    public bool HasRoomFor(ResourceType type, int amount)
    {
        return GetFreeSpace(type) >= amount;
    }

    /// <summary>
    /// Tente d'ajouter une ressource. Retourne false (sans rien modifier) si l'inventaire est plein.
    /// </summary>
    public bool TryAddResource(ResourceType type, int amount)
    {
        if (amount <= 0 || !HasRoomFor(type, amount))
            return false;

        int remaining = amount;

        // 1) On complète d'abord les piles existantes du même type
        for (int i = 0; i < slots.Length && remaining > 0; i++)
        {
            if (slots[i].IsEmpty || slots[i].type != type) continue;

            int added = Mathf.Min(remaining, maxStackSize - slots[i].count);
            slots[i].count += added;
            remaining -= added;
        }

        // 2) Le reste est placé dans les cases vides
        for (int i = 0; i < slots.Length && remaining > 0; i++)
        {
            if (!slots[i].IsEmpty) continue;

            int added = Mathf.Min(remaining, maxStackSize);
            slots[i].type = type;
            slots[i].count = added;
            remaining -= added;
        }

        Debug.Log($"<color=green>[Inventaire] +{amount} {type} ! Total actuel : {GetResourceCount(type)}</color>");
        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Retire des ressources (utile pour le futur système de craft).
    /// Retourne false si le joueur n'en possède pas assez.
    /// </summary>
    public bool TryRemoveResource(ResourceType type, int amount)
    {
        if (amount <= 0 || GetResourceCount(type) < amount)
            return false;

        int remaining = amount;

        for (int i = slots.Length - 1; i >= 0 && remaining > 0; i--)
        {
            if (slots[i].IsEmpty || slots[i].type != type) continue;

            int removed = Mathf.Min(remaining, slots[i].count);
            slots[i].count -= removed;
            remaining -= removed;
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    /// <summary>
    /// Nombre total d'un type de ressource, toutes cases confondues.
    /// </summary>
    public int GetResourceCount(ResourceType type)
    {
        int total = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            if (!slots[i].IsEmpty && slots[i].type == type)
                total += slots[i].count;
        }

        return total;
    }
}