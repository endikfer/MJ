using Unity.Netcode;
using UnityEngine;

public class GunSpawner : NetworkBehaviour
{
    public GameObject gunPrefab;
    public Vector3 spawnPosition = new Vector3(0, 1, 0);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            GameObject gun = Instantiate(gunPrefab, spawnPosition, Quaternion.identity);
            gun.GetComponent<NetworkObject>().Spawn(destroyWithScene: false);

            // Asegurar que el Rigidbody esté listo
            gun.GetComponent<Rigidbody>().isKinematic = false;
        }
    }

    private void SpawnGun()
    {
        GameObject gun = Instantiate(gunPrefab, spawnPosition, Quaternion.identity);
        gun.GetComponent<NetworkObject>().Spawn(destroyWithScene: false);

        // Configuración crítica para VR
        if (gun.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
        }
    }
}