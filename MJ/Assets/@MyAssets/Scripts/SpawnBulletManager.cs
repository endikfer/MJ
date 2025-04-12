using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawnBulletManager : NetworkBehaviour
{
    [SerializeField] private GameObject spawnedObjectPrefab;

    private GameObject spawnedObject;
    private List<NetworkObject> ballList;

    public override void OnNetworkSpawn()
    {
        ballList = new List<NetworkObject>();
        base.OnNetworkSpawn();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnBallRpc(new RpcParams());
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            DespawnBallsRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnBallRpc(RpcParams rpcParams)
    {
        NetworkObject spawnedNetworkObject = NetworkObjectPool.Singleton.GetNetworkObject(spawnedObjectPrefab, transform.position, Quaternion.identity);
        spawnedNetworkObject.SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        ballList.Add(spawnedNetworkObject);

        spawnedNetworkObject.GetComponentInChildren<Rigidbody>().AddForce(Vector3.up * 0.5f);
    }

    [Rpc(SendTo.Server)]
    private void DespawnBallsRpc()
    {
        for (int i = ballList.Count - 1; i >= 0; i--)
        {
            ballList[i].Despawn();
        }

        ballList = new List<NetworkObject>();
    }
}
