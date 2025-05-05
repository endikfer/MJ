using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR;

public class GunShooter : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    void Update()
    {
        if (!IsOwner) return;

        bool rightTriggerPressed = false;
        bool leftMouseClicked = Input.GetMouseButtonDown(0);

        InputDevice rightHand = InputDevices.GetDeviceAtXRNode(XRNode.RightHand);
        if (rightHand.TryGetFeatureValue(CommonUsages.triggerButton, out bool triggerValue))
        {
            rightTriggerPressed = triggerValue;
        }

        if ((rightTriggerPressed || leftMouseClicked) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;

            SpawnBulletRpc(firePoint.position, firePoint.rotation);
        }
    }

    [Rpc(SendTo.Server)]
    private void SpawnBulletRpc(Vector3 position, Quaternion rotation, RpcParams rpcParams = default)
    {
        NetworkObject bulletNetObj = NetworkObjectPool.Singleton.GetNetworkObject(bulletPrefab, position, rotation);
        bulletNetObj.SpawnWithOwnership(rpcParams.Receive.SenderClientId);
        bulletNetObj.GetComponent<BulletLogic>().Init();
    }
}