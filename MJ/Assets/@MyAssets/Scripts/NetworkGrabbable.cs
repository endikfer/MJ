using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkGrabbable : NetworkBehaviour
{
    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
    }

    private void OnDisable()
    {
        grabInteractable.selectEntered.RemoveListener(OnGrab);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (!IsOwner)
        {
            RequestOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);

            var interactorComponent = args.interactorObject as Component;
            if (interactorComponent != null)
            {
                var xrBaseInteractor = interactorComponent.GetComponent<XRBaseInteractor>();
                if (xrBaseInteractor != null && xrBaseInteractor.interactionManager != null)
                {
                    xrBaseInteractor.interactionManager.CancelInteractableSelection(grabInteractable);
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ulong requestingClientId)
    {
        NetworkObject.ChangeOwnership(requestingClientId);
    }
}
