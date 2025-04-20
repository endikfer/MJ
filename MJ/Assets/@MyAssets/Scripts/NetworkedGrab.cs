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
        if (IsServer)
        {
            grab.selectEntered.AddListener(OnGrab);
            grab.selectExited.AddListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRBaseInteractor interactor)
        {
            var root = interactor.transform.root;
            var playerNetObj = root.GetComponent<NetworkObject>();

            if (playerNetObj != null)
            {
                GetComponent<NetworkObject>().ChangeOwnership(playerNetObj.OwnerClientId);
                SetKinematicClientRpc(true);
            }
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        SetKinematicClientRpc(false);
    }

    [ClientRpc]
    void SetKinematicClientRpc(bool isKinematic)
    {
        if (!IsOwner)
        {
            rb.isKinematic = isKinematic;
        }
    }
}
