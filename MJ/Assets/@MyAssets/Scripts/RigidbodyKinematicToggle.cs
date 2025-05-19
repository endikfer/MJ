using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyKinematicToggle : NetworkBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private NetworkVariable<bool> isHeld = new NetworkVariable<bool>(false);

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        isHeld.OnValueChanged += OnIsHeldChanged;
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
        grabInteractable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (!IsOwner) return;
        SetIsHeldServerRpc(true);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (!IsOwner) return;
        SetIsHeldServerRpc(false);
    }

    [ServerRpc]
    private void SetIsHeldServerRpc(bool held)
    {
        isHeld.Value = held;
    }

    private void OnIsHeldChanged(bool previousValue, bool newValue)
    {
        if (newValue)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }
        else
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }
    }
}
