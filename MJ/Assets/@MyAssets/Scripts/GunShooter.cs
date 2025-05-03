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
            
            FireServerRpc(firePoint.position, firePoint.rotation);
        }
    }

    [ServerRpc]
    private void FireServerRpc(Vector3 position, Quaternion rotation)
    {
        if (bulletPrefab == null)
        {
            Debug.LogError("bulletPrefab es NULL en el servidor");
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        var netObj = bullet.GetComponent<NetworkObject>();
        if (netObj == null)
        {
            Debug.LogError("NO hay NetworkObject en el prefab de la bala.");
            return;
        }

        Debug.Log($"Bullet prefab name: {bulletPrefab.name}, has NetworkObject: {bulletPrefab.GetComponent<NetworkObject>() != null}");

        netObj.Spawn();
    }

}