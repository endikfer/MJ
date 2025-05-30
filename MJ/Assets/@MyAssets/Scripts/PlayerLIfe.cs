using UnityEngine;
using Unity.Netcode;

public class PlayerLife : NetworkBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (!IsServer) return;

        currentHealth -= amount;
        Debug.Log($"[Server] Player {OwnerClientId} took {amount} damage, health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }

        UpdateHealthClientRpc(currentHealth);
    }

    public void Heal(float amount)
    {
        if (!IsServer) return;

        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        UpdateHealthClientRpc(currentHealth);
    }

    private void Die()
    {
        Debug.Log($"[Server] Player {OwnerClientId} has died.");
        // Aquí podrías reiniciar la posición, animación, etc.
    }

    [ClientRpc]
    private void UpdateHealthClientRpc(float newHealth)
    {
        currentHealth = newHealth;
    }

    public float GetCurrentHealth() => currentHealth;
}
