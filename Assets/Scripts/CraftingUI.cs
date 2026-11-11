using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Menu de fabrication : s'ouvre et se ferme avec la touche F.
/// - Menu ouvert : curseur libre et visible, MouseLook désactivé.
/// - Menu fermé  : curseur verrouillé et masqué, MouseLook réactivé.
/// Les recettes sont lues directement dans CraftingSystem.Recipes (aucune liste dupliquée).
///
/// Mise en place dans l'Inspector :
///  - Place ce script sur un objet TOUJOURS actif (ex : le Canvas HUD), jamais sur le panneau
///    qu'il active/désactive, sinon il se désactiverait lui-même.
///  - Crafting System : le Player (trouvé automatiquement si vide)
///  - Inventory       : le Player (trouvé automatiquement si vide)
///  - Mouse Look      : la Main Camera, qui porte MouseLook (trouvé automatiquement si vide)
///  - Other Behaviours To Disable : optionnel, ex : PlayerController pour bloquer les déplacements
///  - Menu Panel      : le panneau (ou Canvas) du menu, désactivé au démarrage par le script
///  - Slots Container : l'objet qui contiendra les lignes (avec un Vertical Layout Group)
///  - Slot Prefab     : le prefab "Recipe Entry Slot" (avec CraftingRecipeSlotUI)
/// </summary>
public class CraftingUI : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private CraftingSystem craftingSystem;
    [SerializeField] private Inventory inventory;

    [Header("Blocage des contrôles quand le menu est ouvert")]
    [Tooltip("Script de rotation de la caméra, désactivé pendant que le menu est ouvert.")]
    [SerializeField] private MouseLook mouseLook;

    [Tooltip("Autres scripts à désactiver menu ouvert (ex : PlayerController pour bloquer les déplacements).")]
    [SerializeField] private Behaviour[] otherBehavioursToDisable;

    [Header("Menu")]
    [Tooltip("Panneau ou Canvas du menu à afficher/masquer. Ne doit PAS être l'objet qui porte ce script.")]
    [SerializeField] private GameObject menuPanel;

    [Tooltip("Parent des lignes de recettes (conseillé : Vertical Layout Group).")]
    [SerializeField] private Transform slotsContainer;

    [Tooltip("Prefab d'une ligne de recette (doit porter CraftingRecipeSlotUI).")]
    [SerializeField] private CraftingRecipeSlotUI slotPrefab;

    [Header("Configuration")]
    [SerializeField] private KeyCode toggleKey = KeyCode.F;

    [Tooltip("Referme le menu juste après une fabrication réussie.")]
    [SerializeField] private bool closeAfterCraft = false;

    // Une ligne d'UI par recette, créées une seule fois au démarrage (pas de Instantiate pendant le jeu)
    private readonly List<CraftingRecipeSlotUI> slots = new List<CraftingRecipeSlotUI>();

    /// <summary>
    /// Indique si le menu est actuellement ouvert.
    /// </summary>
    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (craftingSystem == null) craftingSystem = FindFirstObjectByType<CraftingSystem>();
        if (inventory == null) inventory = FindFirstObjectByType<Inventory>();
        if (mouseLook == null) mouseLook = FindFirstObjectByType<MouseLook>();

        if (!ValidateReferences())
        {
            enabled = false;
            return;
        }

        BuildSlots();
    }

    private void OnEnable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged += HandleInventoryChanged;
    }

    private void OnDisable()
    {
        if (inventory != null)
            inventory.OnInventoryChanged -= HandleInventoryChanged;
    }

    private void Start()
    {
        // État initial : menu fermé, curseur verrouillé
        SetMenuOpen(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
            SetMenuOpen(!IsOpen);
        else if (IsOpen && Input.GetKeyDown(KeyCode.Escape))
            SetMenuOpen(false); // Échap ferme aussi le menu
    }

    /// <summary>
    /// Ouvre ou ferme le menu et applique l'état du curseur et des contrôles.
    /// </summary>
    public void SetMenuOpen(bool open)
    {
        IsOpen = open;
        menuPanel.SetActive(open);

        // Curseur : libre et visible dans le menu, verrouillé et masqué en jeu
        Cursor.lockState = open ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = open;

        // Blocage de la rotation caméra (et des autres scripts choisis) pendant le menu
        if (mouseLook != null)
            mouseLook.enabled = !open;

        foreach (Behaviour behaviour in otherBehavioursToDisable)
        {
            if (behaviour != null)
                behaviour.enabled = !open;
        }

        if (open)
            Refresh();
    }

    /// <summary>
    /// Crée une ligne d'UI pour chaque recette de CraftingSystem.
    /// Appeler cette méthode si la liste des recettes change pendant le jeu.
    /// </summary>
    public void BuildSlots()
    {
        foreach (CraftingRecipeSlotUI slot in slots)
        {
            if (slot != null) Destroy(slot.gameObject);
        }
        slots.Clear();

        IReadOnlyList<CraftingRecipe> recipes = craftingSystem.Recipes;

        for (int i = 0; i < recipes.Count; i++)
        {
            if (recipes[i] == null) continue;

            CraftingRecipeSlotUI slot = Instantiate(slotPrefab, slotsContainer);
            slot.Setup(recipes[i], OnCraftClicked);
            slots.Add(slot);
        }
    }

    /// <summary>
    /// Met à jour l'état de chaque ligne : bouton actif et ligne normale si le joueur
    /// a assez de ressources, sinon bouton inactif et ligne grisée.
    /// </summary>
    public void Refresh()
    {
        for (int i = 0; i < slots.Count; i++)
            slots[i].SetCraftable(craftingSystem.HasIngredients(slots[i].Recipe));
    }

    /// <summary>
    /// Appelé quand l'inventaire change (ramassage, fabrication...).
    /// On ne met à jour que si le menu est visible.
    /// </summary>
    private void HandleInventoryChanged()
    {
        if (IsOpen)
            Refresh();
    }

    /// <summary>
    /// Clic sur le bouton "Fabriquer" d'une recette.
    /// </summary>
    private void OnCraftClicked(CraftingRecipe recipe)
    {
        bool crafted = craftingSystem.TryCraft(recipe);

        // Rafraîchissement immédiat du menu (l'inventaire UI se met à jour via son propre événement)
        Refresh();

        if (crafted && closeAfterCraft)
            SetMenuOpen(false);
    }

    /// <summary>
    /// Vérifie que toutes les références nécessaires sont assignées et signale les oublis.
    /// </summary>
    private bool ValidateReferences()
    {
        bool valid = true;

        if (craftingSystem == null) { Debug.LogError("[CraftingUI] CraftingSystem introuvable !", this); valid = false; }
        if (inventory == null) { Debug.LogError("[CraftingUI] Inventory introuvable !", this); valid = false; }
        if (menuPanel == null) { Debug.LogError("[CraftingUI] 'Menu Panel' non assigné.", this); valid = false; }
        if (slotsContainer == null) { Debug.LogError("[CraftingUI] 'Slots Container' non assigné.", this); valid = false; }
        if (slotPrefab == null) { Debug.LogError("[CraftingUI] 'Slot Prefab' non assigné.", this); valid = false; }

        if (menuPanel != null && menuPanel == gameObject)
        {
            Debug.LogError("[CraftingUI] Ce script ne doit pas être sur le Menu Panel lui-même.", this);
            valid = false;
        }

        return valid;
    }
}