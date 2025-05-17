using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable), typeof(NetworkObject))]
public class NetworkGrabSync : NetworkBehaviour
{
    private XRGrabInteractable grabInteractable;
    private NetworkObject netObject;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        netObject = GetComponent<NetworkObject>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (IsClient && !IsOwner)
        {
            RequestOwnershipServerRpc();
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (IsOwner && netObject.IsSpawned)
        {
            // Opcional: Devolver ownership al servidor
            netObject.RemoveOwnership();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ServerRpcParams rpcParams = default)
    {
        var clientId = rpcParams.Receive.SenderClientId;
        if (IsServer)
        {
            netObject.ChangeOwnership(clientId);
        }
    }
}