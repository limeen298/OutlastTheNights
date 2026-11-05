using TMPro;
using UnityEngine;

/// <summary>
/// Gère l'indication d'interaction affichée en bas de l'écran (ex : "[E] Ramasser").
/// À placer sur un objet TOUJOURS actif (ex : le Canvas), pas sur le texte lui-même.
/// </summary>
public class InteractionPromptUI : MonoBehaviour
{
    [Header("Références")]
    [Tooltip("Texte TextMeshPro affiché au joueur.")]
    [SerializeField] private TMP_Text promptText;

    // Dernier message affiché : évite de réécrire le texte à chaque frame
    private string currentMessage;

    private void Awake()
    {
        // L'indication est cachée au démarrage
        Hide();
    }

    /// <summary>
    /// Affiche l'indication avec le message donné.
    /// </summary>
    public void Show(string message)
    {
        if (!promptText.gameObject.activeSelf)
            promptText.gameObject.SetActive(true);

        if (message != currentMessage)
        {
            currentMessage = message;
            promptText.text = message;
        }
    }

    /// <summary>
    /// Cache l'indication.
    /// </summary>
    public void Hide()
    {
        currentMessage = null;

        if (promptText != null && promptText.gameObject.activeSelf)
            promptText.gameObject.SetActive(false);
    }
}