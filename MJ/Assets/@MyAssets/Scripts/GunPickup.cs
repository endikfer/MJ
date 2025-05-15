using Unity.Netcode;
using UnityEngine;

public class GunPickup : NetworkBehaviour
{
    private NetworkObject netObj;

    private void Awake()
    {
        netObj = GetComponent<NetworkObject>();
    }

    public void PickUp(ulong newOwnerClientId, Transform holdPoint)
    {
        if (!IsServer) return;

        // Cambia el ownership al jugador que recoge
        netObj.ChangeOwnership(newOwnerClientId);

        // Hace que el arma sea hija del holdPoint del jugador y reinicia transform
        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // Puedes desactivar física aquí si tienes Rigidbody
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    public void Drop(Vector3 dropPosition)
    {
        if (!IsServer) return;

        // Deja el arma libre en el mapa con ownership del servidor (ClientId 0)
        netObj.RemoveOwnership();
        transform.SetParent(null);
        transform.position = dropPosition;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = false;
    }
}