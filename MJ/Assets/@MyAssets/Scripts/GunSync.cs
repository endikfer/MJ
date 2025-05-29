using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
public class GunSync : NetworkBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private NetworkVariable<ulong> currentOwnerId = new NetworkVariable<ulong>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void Start()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (!IsSpawned) return;

        RequestOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
        UpdateGrabState(true);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (IsOwner)
        {
            ReleaseOwnershipServerRpc();
        }

        UpdateGrabState(false);
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ulong clientId)
    {
        NetworkObject.ChangeOwnership(clientId);
        currentOwnerId.Value = clientId;
    }

    [ServerRpc]
    private void ReleaseOwnershipServerRpc()
    {
        NetworkObject.ChangeOwnership(0);
        currentOwnerId.Value = 0;
    }

    private void UpdateGrabState(bool isGrabbed)
    {
        rb.isKinematic = isGrabbed;
        grabInteractable.throwOnDetach = !isGrabbed;

        if (!isGrabbed)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
