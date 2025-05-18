using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable), typeof(NetworkObject), typeof(Rigidbody))]
public class GunOwnershipHandler : NetworkBehaviour
{
    private XRGrabInteractable grabInteractable;
    private NetworkObject netObj;
    private Rigidbody rb;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        netObj = GetComponent<NetworkObject>();
        rb = GetComponent<Rigidbody>();

        // Configuración crítica para VR + Netcode
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
        grabInteractable.throwOnDetach = false;
        rb.isKinematic = false; // ¡IMPORTANTE!
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (!IsOwner)
        {
            // Solo el cliente que agarra solicita ownership
            RequestOwnershipServerRpc(NetworkManager.LocalClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ulong clientId)
    {
        // El servidor asigna ownership y notifica al cliente
        netObj.ChangeOwnership(clientId);
        NotifyOwnershipClientRpc(clientId);
    }

    [ClientRpc]
    private void NotifyOwnershipClientRpc(ulong newOwnerId)
    {
        if (NetworkManager.LocalClientId == newOwnerId)
        {
            // Fuerza la actualización del grab en el cliente
            grabInteractable.enabled = false;
            grabInteractable.enabled = true;
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (IsOwner)
        {
            // Opcional: Devolver ownership al servidor
            netObj.RemoveOwnership();
        }
    }
}