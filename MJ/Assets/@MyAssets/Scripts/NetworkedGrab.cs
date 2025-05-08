using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkedGrab : NetworkBehaviour
{
    private XRGrabInteractable grab;
    private Rigidbody rb;

    private void Awake()
    {
        grab = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log($"[NetworkedGrab] Grab triggered by {NetworkManager.Singleton.LocalClientId}");
        if (args.interactorObject is XRBaseInteractor interactor)
        {
            var root = interactor.transform.root;
            var playerNetObj = root.GetComponent<NetworkObject>();

            if (playerNetObj != null)
            {
                RequestOwnershipServerRpc(playerNetObj.OwnerClientId);
                SetKinematicClientRpc(true);
            }
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        SetKinematicClientRpc(false);

        if (IsOwner && IsClient)
        {
            ReleaseOwnershipServerRpc();
        }
    }

    [ServerRpc]
    private void RequestOwnershipServerRpc(ulong newOwnerClientId)
    {
        GetComponent<NetworkObject>().ChangeOwnership(newOwnerClientId);
    }

    [ClientRpc]
    private void SetKinematicClientRpc(bool isKinematic)
    {
        rb.isKinematic = isKinematic;        
    }

    [ServerRpc]
    private void ReleaseOwnershipServerRpc()
    {
        NetworkObject.RemoveOwnership();
    }
}