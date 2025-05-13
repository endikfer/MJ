using System.Collections;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class CanvasManager : MonoBehaviour
{
    public Slider healthSlider;
    public TextMeshProUGUI healthText;
    public Image fillImage;

    private PlayerLIfe player;
    private bool playerFound = false;

    void Start()
    {
        StartCoroutine(FindLocalPlayer());
    }

    IEnumerator FindLocalPlayer()
    {
        while (!playerFound)
        {
            PlayerLIfe[] allPlayers = FindObjectsOfType<PlayerLIfe>();
            foreach (var pl in allPlayers)
            {
                if (pl.IsOwner) // Solo el jugador local
                {
                    player = pl;
                    playerFound = true;

                    if (healthSlider != null)
                    {
                        healthSlider.maxValue = player.maxHealth;
                        healthSlider.value = player.maxHealth;
                    }

                    break;
                }
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    void Update()
    {
        if (playerFound && player != null)
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
