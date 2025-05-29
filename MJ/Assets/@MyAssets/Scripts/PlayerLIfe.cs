using UnityEngine;
using Unity.Netcode;

public class PlayerLife : NetworkBehaviour
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
        if (!IsServer) return;

        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }

        UpdateHealthClientRpc(currentHealth);
    }

    public void Heal(float amount)
    {
        if (!IsServer) return;

        currentHealth += amount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        UpdateHealthClientRpc(currentHealth);
    }

    public void Die()
    {
        animator.Die();
        if (IsOwner)
        {
            NotifyDeathToServerRpc();
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.M)) TakeDamage(10);
        if (Input.GetKeyDown(KeyCode.N)) Heal(10);
#endif
    }

    [ClientRpc]
    void UpdateHealthClientRpc(float newHealth)
    {
        currentHealth = newHealth;
        Debug.Log($"[CLIENT RPC] Nueva vida: {currentHealth}");
        // Aquí puedes actualizar una UI si es necesario
    }

    [Rpc(SendTo.Server)]
    private void NotifyDeathToServerRpc()
    {
        FullGameManager.Instance.HandlePlayerDeathServerRpc(OwnerClientId);
    }
}
