using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Logique de fabrication : vérifie les ressources, les consomme et place l'objet devant le joueur.
/// Ce script ne gère PAS l'interface : c'est CraftingUI qui affiche le menu et appelle TryCraft().
///
/// Mise en place dans l'Inspector (sur le GameObject du joueur, à côté de Inventory) :
///  - Inventory       : trouvé automatiquement si laissé vide
///  - Notification UI : le Canvas qui porte NotificationUI (optionnel)
///  - Recipes         : la liste des recettes disponibles (une seule fois, l'UI la lit ici)
/// </summary>
public class CraftingSystem : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Inventaire du joueur (trouvé automatiquement si vide).")]
    [SerializeField] private Inventory inventory;
    [SerializeField] private NotificationUI notificationUI;

    [Header("Déplacement")]
    [Tooltip("Caméra principale pour viser les objets (trouvée automatiquement si vide).")]
    [SerializeField] private Transform playerCamera;

    [Tooltip("Touche pour attraper un objet.")]
    [SerializeField] private KeyCode moveKey = KeyCode.R;

    [Tooltip("Layer assigné aux objets fabriqués.")]
    [SerializeField] private LayerMask craftedObjectLayer;

    private Transform objectToMove; // L'objet actuellement tenu

    [Header("Recettes disponibles")]
    [Tooltip("Liste unique des recettes : le menu de craft (CraftingUI) la lit directement ici.")]
    [SerializeField] private CraftingRecipe[] recipes;

    [Header("Placement de l'objet fabriqué")]
    [Tooltip("Distance devant le joueur où l'objet est placé (en mètres).")]
    [SerializeField, Min(0.5f)] private float placementDistance = 2.5f;

    [Tooltip("Layers considérés comme du sol.")]
    [SerializeField] private LayerMask groundMask = Physics.DefaultRaycastLayers;

    /// <summary>
    /// Recettes disponibles (lecture seule), utilisées par l'UI.
    /// </summary>
    public IReadOnlyList<CraftingRecipe> Recipes => recipes;

    private void Awake()
    {
        if (inventory == null)
            inventory = GetComponent<Inventory>();

        if (inventory == null)
            inventory = FindFirstObjectByType<Inventory>();
    }

    /// <summary>
    /// Tente de fabriquer l'objet d'une recette. Retourne true si la fabrication a réussi.
    /// </summary>
    public bool TryCraft(CraftingRecipe recipe)
    {
        if (recipe == null) return false;

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
        //    -> chaque retrait déclenche Inventory.OnInventoryChanged : l'UI se met à jour
        foreach (CraftingRecipe.Ingredient ingredient in recipe.ingredients)
            inventory.TryRemoveResource(ingredient.type, ingredient.amount);

        // 4) On crée l'objet dans le monde, orienté comme le joueur
        // 4) On crée l'objet dans le monde, avec la rotation du joueur + l'ajustement de la recette
        Quaternion spawnRotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * Quaternion.Euler(recipe.rotationOffset);
        Instantiate(recipe.resultPrefab, point, spawnRotation);


        Notify($"{recipe.recipeName} fabriqué !");
        return true;
    }

    /// <summary>
    /// Vérifie que l'inventaire contient tous les ingrédients de la recette.
    /// Public : l'UI s'en sert pour griser les recettes impossibles.
    /// </summary>
    public bool HasIngredients(CraftingRecipe recipe)
    {
        if (recipe == null || inventory == null) return false;

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

    private void Update()
    {
        if (playerCamera == null) playerCamera = Camera.main.transform;

        // 1. Si on tient un objet, on met à jour sa position
        if (objectToMove != null)
        {
            if (TryGetPlacementPoint(out Vector3 point))
            {
                objectToMove.position = point; // Suit le sol

                // Rotation optionnelle : on garde l'objet aligné avec le joueur pendant le déplacement
                // (Modifiez cette ligne si vous voulez garder la rotation d'origine de l'objet)
                objectToMove.rotation = Quaternion.Euler(0f, transform.eulerAngles.y, 0f);
            }

            // Relâcher l'objet au clic gauche
            if (Input.GetMouseButtonDown(0))
            {
                objectToMove = null;
                Notify("Objet repositionné.");
            }
            return; // Bloque le reste des actions pendant qu'on déplace
        }

        // 2. Si on ne tient rien, on cherche à attraper avec la touche définie (R)
        if (Input.GetKeyDown(moveKey))
        {
            if (Physics.Raycast(playerCamera.position, playerCamera.forward, out RaycastHit hit, 5f, craftedObjectLayer))
            {
                objectToMove = hit.transform;
                Notify("Objet saisi. Clic gauche pour placer.");
            }
        }
    }
}