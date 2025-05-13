using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerLIfe : NetworkBehaviour
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
        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // Notifica a todos los clientes que actualicen su valor local de vida
        UpdateHealthClientRpc(currentHealth);
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

    [ClientRpc]
    void UpdateHealthClientRpc(float newHealth)
    {
        currentHealth = newHealth;

        // Aquí puedes actualizar UI si tienes barra de vida
        Debug.Log($"[CLIENT RPC] Nueva vida: {currentHealth}");
    }
}
