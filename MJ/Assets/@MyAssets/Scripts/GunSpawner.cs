using Unity.Netcode;
using UnityEngine;

public class GunSpawner : NetworkBehaviour
{
    public GameObject gunPrefab;
    public Vector3 spawnPosition = new Vector3(0, 1, 0);  // Ajusta la posición de spawn

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            GameObject gun = Instantiate(gunPrefab, spawnPosition, Quaternion.identity);
            NetworkObject netObj = gun.GetComponent<NetworkObject>();
            netObj.Spawn();  // Spawn sin asignar ownership (queda en servidor)
        }
    }
}
    