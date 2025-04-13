using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLIfe : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(float amount)
    {
        currentHealth += amount;
    }

    public void Die()
    {

    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    void Update()
    {
        // Ejemplo: Presiona 'm' para recibir daño, 'n' para curarse
        if (Input.GetKeyDown(KeyCode.M))
        {
            TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.N))
        {
            Heal(10);
        }
    }
}
