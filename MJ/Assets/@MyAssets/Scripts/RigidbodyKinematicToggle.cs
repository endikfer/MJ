using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyKinematicToggle : NetworkBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
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

        Debug.Log("GRAB → Activando kinematic");
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (!IsOwner) return;

        Debug.Log("RELEASE → Desactivando kinematic");
        rb.isKinematic = false;
        rb.useGravity = true;
    }
}
