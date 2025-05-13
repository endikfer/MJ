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
            PlayerLIfe vida = other.GetComponent<PlayerLIfe>() ?? other.GetComponentInParent<PlayerLIfe>();
            if (vida == null)
            {
                Debug.LogWarning("No se encontró PlayerLife en " + other.name + " ni en sus padres.");
            }
            else
            {
                Debug.Log("Se encontró PlayerLife en " + vida.gameObject.name);
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
