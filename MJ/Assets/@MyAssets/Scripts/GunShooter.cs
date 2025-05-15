using Unity.Netcode;
using UnityEngine;

public class GunShooter : NetworkBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 0.5f;
    private float nextFireTime = 0f;

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetMouseButtonDown(0) && Time.time >= nextFireTime)
        {
            nextFireTime = Time.time + fireRate;
            SpawnBulletServerRpc(firePoint.position, firePoint.rotation);
        }
    }

    [ServerRpc]
    private void SpawnBulletServerRpc(Vector3 position, Quaternion rotation)
    {
        GameObject bullet = Instantiate(bulletPrefab, position, rotation);
        NetworkObject netObj = bullet.GetComponent<NetworkObject>();
        netObj.Spawn();
    }
}