using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Un élément du menu de craft (une ligne par recette).
/// À placer sur la RACINE du prefab "Recipe Entry Slot".
///
/// Mise en place dans l'Inspector du prefab :
///  - Name Text    : TextMeshPro qui affiche le nom de la recette
///  - Cost Text    : TextMeshPro qui affiche le coût (ex : "2 Wood + 1 Stone")
///  - Icon Image   : Image qui affiche l'aperçu de la recette
///  - Craft Button : le bouton "Fabriquer"
///  - Canvas Group : CanvasGroup de la racine (sert à griser toute la ligne)
/// </summary>
public class CraftingRecipeSlotUI : MonoBehaviour
{
    [Header("Références (dans le prefab)")]
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text costText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button craftButton;

    [Tooltip("CanvasGroup de la racine : l'alpha est réduit quand la recette est impossible.")]
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Apparence")]
    [Tooltip("Transparence de la ligne quand le joueur n'a pas assez de ressources.")]
    [SerializeField, Range(0f, 1f)] private float unavailableAlpha = 0.5f;

    /// <summary>
    /// Recette représentée par cet élément.
    /// </summary>
    public CraftingRecipe Recipe { get; private set; }

    /// <summary>
    /// Remplit l'élément avec les informations de la recette.
    /// 'onCraftClicked' est appelé quand le joueur clique sur le bouton.
    /// </summary>
    public void Setup(CraftingRecipe recipe, Action<CraftingRecipe> onCraftClicked)
    {
        Recipe = recipe;

        nameText.text = recipe.recipeName;
        costText.text = recipe.GetCostText(); // Le coût ne change pas : calculé une seule fois

        if (iconImage != null)
        {
            iconImage.sprite = recipe.icon;
            iconImage.enabled = recipe.icon != null; // Pas d'icône : on cache l'Image
        }

        craftButton.onClick.RemoveAllListeners();
        craftButton.onClick.AddListener(() => onCraftClicked(recipe));
    }

    /// <summary>
    /// Active le bouton et affiche la ligne normalement, ou la grise si la recette est impossible.
    /// </summary>
    public void SetCraftable(bool craftable)
    {
        craftButton.interactable = craftable;

        if (canvasGroup != null)
            canvasGroup.alpha = craftable ? 1f : unavailableAlpha;
    }
}