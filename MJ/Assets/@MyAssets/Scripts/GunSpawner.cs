using Unity.Netcode;
using UnityEngine;
using XRMultiplayer;

public class GunSpawner : NetworkBehaviour
{
    public GameObject gunPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            Debug.Log($"[GunSpawner] Spawning gun for client {OwnerClientId}");
            SpawnGunForClient(OwnerClientId);
        }
    }

    private void SpawnGunForClient(ulong clientId)
    {
        Debug.Log($"[GunSpawner] Instantiating gunPrefab for client {clientId}");

        if (gunPrefab == null)
        {
            Debug.LogError("[GunSpawner] gunPrefab is NULL!");
            return;
        }

        GameObject gun = Instantiate(gunPrefab, GetSpawnPosition(clientId), Quaternion.identity);
        NetworkObject netObj = gun.GetComponent<NetworkObject>();

        if (netObj == null)
        {
            Debug.LogError("[GunSpawner] Missing NetworkObject on gunPrefab!");
            return;
        }

        netObj.SpawnWithOwnership(clientId);
        netObj.DontDestroyWithOwner = true;

        if (gun.TryGetComponent<NetworkPhysicsInteractable>(out var interactable))
        {
            interactable.spawnLocked = false;
        }
    }

    private Vector3 GetSpawnPosition(ulong clientId)
    {
        if (NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out var client))
        {
            if (client.PlayerObject != null)
            {
                return client.PlayerObject.transform.position + Vector3.forward * 1.5f;
            }
        }
        return new Vector3(-1 + (int)clientId, 1, 0);
    }
}