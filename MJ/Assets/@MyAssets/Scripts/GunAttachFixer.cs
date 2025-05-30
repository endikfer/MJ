using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GunAttachFixer : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Transform originalParent;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        originalParent = transform.parent;
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
        // Parentar al transform del interactor (controlador)
        transform.SetParent(args.interactorObject.transform, worldPositionStays: false);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Volver a poner al padre original cuando se suelta
        transform.SetParent(originalParent, worldPositionStays: true);
    }
}
