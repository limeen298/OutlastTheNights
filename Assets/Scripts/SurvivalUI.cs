
using UnityEngine;
using UnityEngine.UI;

public class SurvivalUI : MonoBehaviour
{
    // Référence au script qui gère les statistiques du joueur
    public PlayerStats playerStats;

    // Les quatre barres affichées à l'écran
    public Slider coldBar;
    public Slider hungerBar;
    public Slider thirstBar;
    public Slider healthBar;

    void Update()
    {
        // Vérifier que les références sont bien renseignées
        if (playerStats == null ||
            coldBar == null ||
            hungerBar == null ||
            thirstBar == null ||
            healthBar == null)
        {
            return;
        }

        // Mettre à jour les barres avec les valeurs du joueur
        coldBar.value = playerStats.cold;
        hungerBar.value = playerStats.hunger;
        thirstBar.value = playerStats.thirst;
        healthBar.value = playerStats.health;
    }
}
