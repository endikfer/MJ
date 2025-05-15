using Unity.Netcode;
using UnityEngine;

public class PlayerPickupHandler : NetworkBehaviour
{
    public GameObject gunHoldPoint;  // Punto donde se sujeta el arma
    private GunPickup heldGun;

    void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(KeyCode.E))  // Cambia tecla si quieres
        {
            if (heldGun == null)
            {
                TryPickUpGun();
            }
            else
            {
                DropGun();
            }
        }
    }

    private void TryPickUpGun()
    {
        // Detecta arma cerca usando overlap sphere o trigger (simplificado con raycast aquí)
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 2f))
        {
            GunPickup gun = hit.collider.GetComponent<GunPickup>();
            if (gun != null)
            {
                // Llama al servidor para recoger arma
                PickUpGunServerRpc(gun.NetworkObjectId);
            }
        }
    }

    [ServerRpc]
    private void PickUpGunServerRpc(ulong gunNetworkId)
    {
        NetworkObject gunNetObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[gunNetworkId];
        GunPickup gun = gunNetObj.GetComponent<GunPickup>();

        if (gun != null)
        {
            gun.PickUp(OwnerClientId, gunHoldPoint.transform);
            heldGun = gun;
        }
    }

    private void DropGun()
    {
        if (heldGun == null) return;

        // Llama al servidor para soltar arma
        DropGunServerRpc(heldGun.NetworkObjectId, heldGun.transform.position + transform.forward * 1.5f);
        heldGun = null;
    }

    [ServerRpc]
    private void DropGunServerRpc(ulong gunNetworkId, Vector3 dropPosition)
    {
        NetworkObject gunNetObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[gunNetworkId];
        GunPickup gun = gunNetObj.GetComponent<GunPickup>();

        if (gun != null)
        {
            gun.Drop(dropPosition);
        }
    }
}