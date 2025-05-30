using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GunFollowHand : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Transform followTarget;
    private bool isGrabbed = false;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
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
        followTarget = args.interactorObject.transform;
        isGrabbed = true;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        followTarget = null;
    }

    private void LateUpdate()
    {
        if (isGrabbed && followTarget != null)
        {
            // Mueve y rota la pistola para que coincida con la mano
            transform.position = followTarget.position;
            transform.rotation = followTarget.rotation;
        }
    }
}
