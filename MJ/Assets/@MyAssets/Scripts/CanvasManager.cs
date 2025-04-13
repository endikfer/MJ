using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasManager : MonoBehaviour
{
    /*public Slider healthSlider; // Referencia al slider de vida
    public PlayerLIfe player;   // Referencia al script del jugador


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
            healthSlider.value = player.GetCurrentHealth();
        }
    }*/

    public TextMeshProUGUI healthText; // Referencia al texto TMP
    public PlayerLIfe player;          // Referencia al script del jugador

    void Update()
    {
        Debug.Log("Actualizando vida a: " + player.GetCurrentHealth());
        if (player != null && healthText != null)
        {
            healthText.text = "Vida: " + Mathf.CeilToInt(player.GetCurrentHealth()).ToString();
        }
    }
}
