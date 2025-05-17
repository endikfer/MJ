using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(XRGrabInteractable))]
public class ForceAssignXRManager : MonoBehaviour
{
    void Start()
    {
        XRGrabInteractable grabInteractable = GetComponent<XRGrabInteractable>();
        XRInteractionManager manager = FindObjectOfType<XRInteractionManager>();

        if (grabInteractable.interactionManager == null && manager != null)
        {
            grabInteractable.interactionManager = manager;
            Debug.Log("XR Interaction Manager asignado por código a: " + gameObject.name);
        }
    }
}