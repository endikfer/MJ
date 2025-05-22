using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkGrabbable : NetworkBehaviour
{
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (!IsOwner)
        {
            ulong clientId = NetworkManager.LocalClientId;
            Debug.Log($"Solicitando ownership para cliente {clientId}");
            RequestOwnershipServerRpc(clientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ulong clientId)
    {
        Debug.Log($"[SERVER] Transferencia de ownership al cliente {clientId}");
        NetworkObject.ChangeOwnership(clientId);
        NotifyOwnershipClientRpc(clientId);
    }

    [ClientRpc]
    private void NotifyOwnershipClientRpc(ulong clientId)
    {
        if (NetworkManager.LocalClientId == clientId)
        {
            Debug.Log("Ownership confirmado por el cliente, listo para disparar.");
        }
    }
}
