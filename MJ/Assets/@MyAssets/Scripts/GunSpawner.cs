using Unity.Netcode;
using UnityEngine;

public class GunSpawner : NetworkBehaviour
{
    public GameObject gunPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SpawnGunForClient(OwnerClientId);
        }
    }

    private void SpawnGunForClient(ulong clientId)
    {
        GameObject gun = Instantiate(gunPrefab, GetSpawnPosition(clientId), Quaternion.identity);
        var networkObject = gun.GetComponent<NetworkObject>();
        networkObject.SpawnWithOwnership(clientId);
    }

    private Vector3 GetSpawnPosition(ulong clientId)
    {
        return new Vector3(0, 1, 0);
    }
}