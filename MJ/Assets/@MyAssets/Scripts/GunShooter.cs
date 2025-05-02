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

            Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

            FireServerRpc(firePoint.position, firePoint.rotation);
        }
    }

    [ServerRpc]
    private void FireServerRpc(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        bullet.GetComponent<NetworkObject>().Spawn();
    }
}