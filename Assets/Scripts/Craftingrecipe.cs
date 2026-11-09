using System;
using System.Text;
using UnityEngine;

/// <summary>
/// Recette de fabrication : une liste d'ingrédients -> un objet placé dans le monde.
/// Se crée dans le Project via : clic droit > Create > Survie > Recette de craft.
/// Chaque recette est un fichier .asset : idéal pour travailler à plusieurs avec Git
/// (on ajoute une recette sans toucher à la scène).
/// </summary>
[CreateAssetMenu(fileName = "Recipe_Nouvelle", menuName = "Survie/Recette de craft")]
public class CraftingRecipe : ScriptableObject
{
    /// <summary>
    /// Un ingrédient : un type de ressource et la quantité nécessaire.
    /// </summary>
    [Serializable]
    public struct Ingredient
    {
        public ResourceType type;
        [Min(1)] public int amount;
    }

    [Header("Informations")]
    public string recipeName = "Feu de camp";

    [Tooltip("Touche à appuyer pour fabriquer.")]
    public KeyCode craftKey = KeyCode.F;

    [Header("Fabrication")]
    [Tooltip("Ressources consommées. Un seul élément par type de ressource.")]
    public Ingredient[] ingredients;

    [Tooltip("Objet créé dans le monde (ex : le prefab Campfire).")]
    public GameObject resultPrefab;

    /// <summary>
    /// Texte du coût de la recette, ex : "2 Wood + 1 Stone".
    /// </summary>
    public string GetCostText()
    {
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < ingredients.Length; i++)
        {
            if (i > 0) builder.Append(" + ");
            builder.Append(ingredients[i].amount).Append(' ').Append(ingredients[i].type);
        }

        return builder.ToString();
    }
}