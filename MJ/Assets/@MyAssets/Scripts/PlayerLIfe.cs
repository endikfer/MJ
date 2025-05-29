using UnityEngine;
using Unity.Netcode;

public class PlayerLife : NetworkBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public AnimatorController animator;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (!IsServer) return;

        currentHealth -= amount;
        Debug.Log($"[SERVER] Player {OwnerClientId} took {amount} damage. Health: {currentHealth}");

        if (currentHealth <= 0f)
        {
            Die();
        }

        UpdateHealthClientRpc(currentHealth);
    }

    public void Heal(float amount)
    {
        if (!IsServer) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        UpdateHealthClientRpc(currentHealth);
    }

    private void Die()
    {
        animator?.Die();

        if (IsOwner)
        {
            NotifyDeathToServerRpc();
        }
    }

    [ClientRpc]
    private void UpdateHealthClientRpc(float newHealth)
    {
        currentHealth = newHealth;
        Debug.Log($"[CLIENT {OwnerClientId}] Health updated: {currentHealth}");
    }

    [Rpc(SendTo.Server)]
    private void NotifyDeathToServerRpc()
    {
        Debug.Log($"[SERVER] Player {OwnerClientId} died.");
        FullGameManager.Instance?.HandlePlayerDeathServerRpc(OwnerClientId);
    }

    //Este método es necesario para CanvasManager
    public float GetCurrentHealth()
    {
        return currentHealth;
    }
}

