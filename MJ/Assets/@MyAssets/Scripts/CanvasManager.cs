using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    public Slider healthSlider; // Referencia al slider de vida
    public PlayerLIfe player;   // Referencia al script del jugador
    public TextMeshProUGUI healthText; // Referencia al texto TMP
    public Image fillImage; // Referencia al componente Image del Fill del slider

    void Start()
    {
        if (player != null && healthSlider != null)
        {
            healthSlider.maxValue = player.maxHealth;
            healthSlider.value = player.maxHealth;
        }
    }

    void Update()
    {
        if (player != null && healthSlider != null)
        {
            float currentHealth = player.GetCurrentHealth();
            healthSlider.value = currentHealth;
            healthText.text = "Vida: " + Mathf.CeilToInt(currentHealth).ToString();

            if (fillImage != null && healthText != null)
            {
                float percentage = (currentHealth / player.maxHealth) * 100f;
                Color color;

                if (percentage > 67f)
                {
                    color = Color.green;
                }
                else if (percentage > 33f)
                {
                    color = Color.yellow;
                }
                else
                {
                    color = Color.red;
                }

                fillImage.color = color;
                healthText.color = color;
            }
        }
    }
}
