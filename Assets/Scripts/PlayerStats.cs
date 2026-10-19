using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Jauges de Survie (0 à 100)")]
    public float cold = 100f;   // Froid[cite: 6, 7]
    public float hunger = 100f; // Faim[cite: 6, 7]
    public float thirst = 100f; // Soif[cite: 7]

    public float decreaseRate = 0.2f; // Vitesse de diminution des points

    void Update()
    {
        // Les jauges diminuent progressivement avec le temps
        if (cold > 0) cold -= decreaseRate * Time.deltaTime;
        if (hunger > 0) hunger -= decreaseRate * Time.deltaTime;
        if (thirst > 0) thirst -= decreaseRate * Time.deltaTime;

        // Optionnel : empécher les jauges de descendre sous 0 ou de dépasser 100
        cold = Mathf.Clamp(cold, 0f, 100f);
        hunger = Mathf.Clamp(hunger, 0f, 100f);
        thirst = Mathf.Clamp(thirst, 0f, 100f);
    }
}
