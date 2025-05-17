using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable), typeof(NetworkObject))]
public class GunOwnershipHandler : NetworkBehaviour
{
    private XRGrabInteractable grabInteractable;
    private NetworkObject netObj;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        netObj = GetComponent<NetworkObject>();

        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);

        // Asegurarse de que el XRGrabInteractable puede ser interactuado por todos
        grabInteractable.interactionLayers = InteractionLayerMask.GetMask("Default", "Interactable");
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (!IsOwner) // Solo solicitar propiedad si no somos los dueños
        {
            RequestOwnershipServerRpc();
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (IsOwner)
        {
            ReleaseOwnershipServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ServerRpcParams rpcParams = default)
    {
        netObj.ChangeOwnership(rpcParams.Receive.SenderClientId);
    }

    [ServerRpc]
    private void ReleaseOwnershipServerRpc()
    {
        netObj.RemoveOwnership();
    }
}