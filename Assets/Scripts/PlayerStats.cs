
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    [Header("Jauges de Survie (0 à 100)")]
    public float cold = 100f;
    public float hunger = 100f;
    public float thirst = 100f;

    public float decreaseRate = 0.2f;

    // Indique si le joueur se trouve dans une zone réchauffante
    public bool isHeating = false;

    void Update()
    {
        // Le froid diminue uniquement si le joueur est loin du feu
        if (cold > 0 && !isHeating)
        {
            cold -= decreaseRate * Time.deltaTime;
        }

        // La faim et la soif diminuent toujours
        if (hunger > 0)
            hunger -= decreaseRate * Time.deltaTime;

        if (thirst > 0)
            thirst -= decreaseRate * Time.deltaTime;

        // Les valeurs restent entre 0 et 100
        cold = Mathf.Clamp(cold, 0f, 100f);
        hunger = Mathf.Clamp(hunger, 0f, 100f);
        thirst = Mathf.Clamp(thirst, 0f, 100f);
    }
}
