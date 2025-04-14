using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnBulletManager : NetworkBehaviour
{
    [SerializeField] private GameObject spawnedObjectPrefab;
    [SerializeField] private InputActionAsset inputActions;

    private GameObject spawnedObject;
    private List<NetworkObject> bulletList;
    private InputAction rightTriggerAction;

    public override void OnNetworkSpawn()
    {
        bulletList = new List<NetworkObject>();
        base.OnNetworkSpawn();

        var vrMap = inputActions.FindActionMap("VRControls", true);
        rightTriggerAction = vrMap.FindAction("RightTrigger", true);
        rightTriggerAction.Enable();
    }

    private void OnDisable()
    {
        if (rightTriggerAction != null) rightTriggerAction.Disable();
    }

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.K))
        {
            SpawnBulletRpc(new RpcParams());
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            DespawnBulletsRpc();
        }

        if (rightTriggerAction != null && rightTriggerAction.WasPressedThisFrame())
        {
            SpawnBulletRpc(new RpcParams());
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnBulletRpc(RpcParams rpcParams)
    {
        NetworkObject spawnedNetworkObject = NetworkObjectPool.Singleton.GetNetworkObject(spawnedObjectPrefab, transform.position, Quaternion.identity);
        spawnedNetworkObject.SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        bulletList.Add(spawnedNetworkObject);

        spawnedNetworkObject.GetComponentInChildren<Rigidbody>().AddForce(Vector3.up * 0.5f);
    }

    [Rpc(SendTo.Server)]
    private void DespawnBulletsRpc()
    {
        for (int i = bulletList.Count - 1; i >= 0; i--)
        {
            bulletList[i].Despawn();
        }

        bulletList = new List<NetworkObject>();
    }
}
