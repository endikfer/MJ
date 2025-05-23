using UnityEngine;
using Unity.Netcode;

public class HealingItem : NetworkBehaviour
{
    public int healAmount = 100;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("ha entrado " + other.tag);
        if (!IsServer) return; // solo el servidor procesa la colisión

        if (other.CompareTag("Player"))
        {
            Debug.Log("Entró: " + other.name + " | IsServer: " + IsServer + " | IsHost: " + NetworkManager.Singleton.IsHost);
            PlayerLife vida = other.GetComponent<PlayerLife>() ?? other.GetComponentInParent<PlayerLife>();
            if (vida != null)
            {
                vida.Heal(50);
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
