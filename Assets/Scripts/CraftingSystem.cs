using UnityEngine;

/// <summary>
/// Système de fabrication : quand le joueur appuie sur la touche d'une recette,
/// on vérifie ses ressources, on les consomme et on place l'objet devant lui.
/// À placer sur le GameObject du joueur (à côté de Inventory).
/// </summary>
public class CraftingSystem : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Inventaire du joueur (trouvé automatiquement si vide).")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private NotificationUI notificationUI;

    [Header("Recettes disponibles")]
    [SerializeField] private CraftingRecipe[] recipes;

    [Header("Placement de l'objet fabriqué")]
    [Tooltip("Distance devant le joueur où l'objet est placé (en mètres).")]
    [SerializeField, Min(0.5f)] private float placementDistance = 2.5f;

    [Tooltip("Layers considérés comme du sol.")]
    [SerializeField] private LayerMask groundMask = Physics.DefaultRaycastLayers;

    private void Awake()
    {
        if (inventory == null)
            inventory = GetComponent<Inventory>();

        if (inventory == null)
            inventory = FindFirstObjectByType<Inventory>();
    }

    private void Update()
    {
        // Une touche par recette (la liste est courte : aucun impact sur les performances)
        for (int i = 0; i < recipes.Length; i++)
        {
            if (recipes[i] != null && Input.GetKeyDown(recipes[i].craftKey))
                TryCraft(recipes[i]);
        }
    }

    /// <summary>
    /// Tente de fabriquer l'objet d'une recette. Retourne true si la fabrication a réussi.
    /// </summary>
    public bool TryCraft(CraftingRecipe recipe)
    {
        if (recipe.resultPrefab == null)
        {
            Debug.LogError($"[Craft] La recette '{recipe.name}' n'a pas de Result Prefab !", recipe);
            return false;
        }

        // 1) Le joueur a-t-il assez de ressources ?
        if (!HasIngredients(recipe))
        {
            Notify($"Il manque des ressources : {recipe.GetCostText()}");
            return false;
        }

        // 2) Y a-t-il un sol correct devant le joueur ?
        if (!TryGetPlacementPoint(out Vector3 point))
        {
            Notify("Impossible de placer ça ici.");
            return false;
        }

        // 3) On consomme les ressources (déjà vérifiées à l'étape 1)
        foreach (CraftingRecipe.Ingredient ingredient in recipe.ingredients)
            inventory.TryRemoveResource(ingredient.type, ingredient.amount);

        // 4) On crée l'objet dans le monde, orienté comme le joueur
        Instantiate(recipe.resultPrefab, point, Quaternion.Euler(0f, transform.eulerAngles.y, 0f));

        Notify($"{recipe.recipeName} fabriqué !");
        return true;
    }

    /// <summary>
    /// Vérifie que l'inventaire contient tous les ingrédients de la recette.
    /// </summary>
    private bool HasIngredients(CraftingRecipe recipe)
    {
        foreach (CraftingRecipe.Ingredient ingredient in recipe.ingredients)
        {
            if (inventory.GetResourceCount(ingredient.type) < ingredient.amount)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Cherche le sol devant le joueur : on part d'un point en hauteur devant lui
    /// et on lance un rayon vers le bas. Refuse les pentes trop raides (murs, rochers).
    /// </summary>
    private bool TryGetPlacementPoint(out Vector3 point)
    {
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, Vector3.up).normalized;
        Vector3 start = transform.position + forward * placementDistance + Vector3.up * 2f;

        if (Physics.Raycast(start, Vector3.down, out RaycastHit hit, 6f, groundMask, QueryTriggerInteraction.Ignore)
            && hit.normal.y > 0.7f)
        {
            point = hit.point;
            return true;
        }

        point = Vector3.zero;
        return false;
    }

    /// <summary>
    /// Affiche un message au joueur (si l'UI est assignée) et l'écrit dans la Console.
    /// </summary>
    private void Notify(string message)
    {
        Debug.Log($"[Craft] {message}");
        notificationUI?.Show(message);
    }
}