using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerLIfe : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public AnimatorController animator;

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
        if (currentHealth > maxHealth)
        {
            currentHealth += amount;
        }
        if(currentHealth <= maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    public void Die()
    {
        animator.Die();
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
