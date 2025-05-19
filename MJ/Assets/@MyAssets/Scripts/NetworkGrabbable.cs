using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(NetworkObject), typeof(XRGrabInteractable))]
public class NetworkGrabbable : MonoBehaviour
{
    private XRGrabInteractable grabInteractable;

    void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnSelectEntered);
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Solo el host puede transferir ownership
        if (!NetworkManager.Singleton.IsServer)
            return;

        var interactorObject = args.interactorObject;
        var interactorGO = interactorObject.transform.gameObject;

        var networkObject = GetComponent<NetworkObject>();

        // Obtenemos el clientId del jugador que está agarrando
        ulong clientId = interactorGO.GetComponent<NetworkObject>()?.OwnerClientId ?? 0;

        // Transferimos ownership al jugador que agarró
        networkObject.ChangeOwnership(clientId);
    }
}
