using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Affiche un message temporaire à l'écran (ex : "Feu de camp fabriqué !").
/// À placer sur un objet TOUJOURS actif (ex : le Canvas), pas sur le texte lui-même.
/// </summary>
public class NotificationUI : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private TMP_Text messageText;

    [Header("Configuration")]
    [Tooltip("Durée d'affichage du message (en secondes).")]
    [SerializeField, Min(0.5f)] private float displayDuration = 2.5f;

    private WaitForSeconds waitDelay;
    private Coroutine hideRoutine;

    private void Awake()
    {
        // Créé une seule fois pour éviter d'allouer de la mémoire à chaque message
        waitDelay = new WaitForSeconds(displayDuration);
        messageText.gameObject.SetActive(false);
    }

    /// <summary>
    /// Affiche un message, puis le cache automatiquement après quelques secondes.
    /// </summary>
    public void Show(string message)
    {
        messageText.text = message;
        messageText.gameObject.SetActive(true);

        // Si un message est déjà affiché, on relance le compte à rebours
        if (hideRoutine != null)
            StopCoroutine(hideRoutine);

        hideRoutine = StartCoroutine(HideAfterDelay());
    }

    private IEnumerator HideAfterDelay()
    {
        yield return waitDelay;
        messageText.gameObject.SetActive(false);
        hideRoutine = null;
    }
}