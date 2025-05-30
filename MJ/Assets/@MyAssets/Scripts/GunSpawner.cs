using Unity.Netcode;
using UnityEngine;
using System.Collections;


public class GunSpawner : NetworkBehaviour
{
    public GameObject gunPrefab1;
    public GameObject gunPrefab2;

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
        GameObject gun1 = Instantiate(gunPrefab1, new Vector3(1.86f, 1.51f, -6.96f), Quaternion.identity);
        GameObject gun2 = Instantiate(gunPrefab2, new Vector3(-1.795f, 1.528f, 6.981f), Quaternion.identity);
        gun1.GetComponent<NetworkObject>().Spawn();
        gun2.GetComponent<NetworkObject>().Spawn();

    }
}
