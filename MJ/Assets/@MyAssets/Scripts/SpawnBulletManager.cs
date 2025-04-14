using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class SpawnBulletManagerVR : NetworkBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletForce = 10f;
    [SerializeField] private Transform firePoint;

    [SerializeField] private InputActionAsset inputActions;

    private InputAction rightTriggerAction;
    private List<NetworkObject> bulletList;
    private float cooldown = 0.25f;
    private float lastShootTime;

    public override void OnNetworkSpawn()
    {
        bulletList = new List<NetworkObject>();

        var vrMap = inputActions.FindActionMap("XRControls", true);
        rightTriggerAction = vrMap.FindAction("RightTrigger", true);
        rightTriggerAction.Enable();
    }

    private void OnDisable()
    {
        if (rightTriggerAction != null)
            rightTriggerAction.Disable();
    }

    private void Update()
    {
        if (!IsOwner) return;

        bool triggerPressed = rightTriggerAction != null && rightTriggerAction.WasPressedThisFrame();
        bool HPressed = Input.GetKeyDown(KeyCode.H);

        if ((triggerPressed || HPressed) && Time.time - lastShootTime >= cooldown)
        {
            lastShootTime = Time.time;
            SpawnBulletServerRpc(firePoint.position, firePoint.forward);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            DespawnBulletsServerRpc();
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnBulletServerRpc(Vector3 position, Vector3 direction)
    {
        Debug.Log($"[Server] Spawning bullet at {position}");
        var bulletObject = NetworkObjectPool.Singleton.GetNetworkObject(bulletPrefab, position, Quaternion.LookRotation(direction));
        bulletObject.Spawn();

        bulletList.Add(bulletObject);

        Rigidbody rb = bulletObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * bulletForce;
        }
    }

    [Rpc(SendTo.Server)]
    private void DespawnBulletsServerRpc()
    {
        foreach (var bullet in bulletList)
        {
            if (bullet != null && bullet.IsSpawned)
            {
                bullet.Despawn();
            }
        }

        bulletList.Clear();
    }
}
