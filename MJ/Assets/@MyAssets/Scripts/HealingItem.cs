using UnityEngine;
using Unity.Netcode;

public class HealingItem : NetworkBehaviour
{
    public int healAmount = 100;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // solo el servidor procesa la colisión

        if (other.CompareTag("Player"))
        {
            PlayerLIfe vida = other.GetComponent<PlayerLIfe>();
            if (vida != null)
            {
                vida.Heal(healAmount); // opcional: podrías hacer esto con RPC si el jugador no es host
            }

            HealingSpawner.Instance.OnHealingItemCollected();

            GetComponent<NetworkObject>().Despawn(true);
        }
    }

    void Update()
    {
        transform.Rotate(0f, 90f * Time.deltaTime, 0f);
    }
}
