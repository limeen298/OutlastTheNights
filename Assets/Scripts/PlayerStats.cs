

using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [Header("Jauges de Survie (0 à 100)")]
    public float cold = 100f;
    public float hunger = 100f;
    public float thirst = 100f;
    public float health = 100f;

    public float decreaseRate = 0.2f;

    public bool isHeating = false;

    [Header("Interface de santé")]
    public Slider healthBar;

    void Start()
    {
        UpdateHealthBar();
    }

    void Update()
    {
        if (cold > 0 && !isHeating)
        {
            cold -= decreaseRate * Time.deltaTime;
        }

        if (hunger > 0)
        {
            hunger -= decreaseRate * Time.deltaTime;
        }

        if (thirst > 0)
        {
            thirst -= decreaseRate * Time.deltaTime;
        }

        cold = Mathf.Clamp(cold, 0f, 100f);
        hunger = Mathf.Clamp(hunger, 0f, 100f);
        thirst = Mathf.Clamp(thirst, 0f, 100f);
        health = Mathf.Clamp(health, 0f, 100f);

        UpdateHealthBar();
    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Clamp(health - damage, 0f, 100f);
        UpdateHealthBar();
    }

    public void RestoreHunger(float amount)
    {
        hunger = Mathf.Clamp(hunger + amount, 0f, 100f);
    }

    void UpdateHealthBar()
    {
        if (healthBar != null)
        {
            healthBar.value = health;
        }
    }
}
