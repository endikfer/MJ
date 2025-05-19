using Unity.Netcode;
using UnityEngine;
using System.Collections;


public class GunSpawner : NetworkBehaviour
{
    public GameObject gunPrefab;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            StartCoroutine(WaitForClientsAndSpawn());
        }
    }

    private IEnumerator WaitForClientsAndSpawn()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton.ConnectedClientsList.Count > 1);
        SpawnGun();
    }

    private void SpawnGun()
    {
        GameObject gun = Instantiate(gunPrefab, new Vector3(0, 1, 0), Quaternion.identity);
        gun.GetComponent<NetworkObject>().Spawn();
    }
}
