using Unity.Netcode;
using UnityEngine;

public class GunSpawner : NetworkBehaviour
{
    public GameObject gunPrefab;
    public Vector3 spawnPosition = new Vector3(0, 1, 0);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            GameObject gun = Instantiate(gunPrefab, spawnPosition, Quaternion.identity);
            gun.GetComponent<NetworkObject>().Spawn(); // No ownership asignado
        }
    }
}