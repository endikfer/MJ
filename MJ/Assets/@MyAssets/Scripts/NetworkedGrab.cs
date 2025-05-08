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
        // Asegúrate de que los listeners se agreguen en todos los clientes
        grab.selectEntered.AddListener(OnGrab);
        grab.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log("OnGrab called");
        if (args.interactorObject is XRBaseInteractor interactor)
        {
            var root = interactor.transform.root;
            var playerNetObj = root.GetComponent<NetworkObject>();

            if (playerNetObj != null)
            {
                Debug.Log("Requesting ownership");
                RequestOwnershipServerRpc(playerNetObj.OwnerClientId);
                SetKinematicClientRpc(true);
            }
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        Debug.Log("OnRelease called");
        SetKinematicClientRpc(false);
    }

    [ServerRpc]
    private void RequestOwnershipServerRpc(ulong newOwnerClientId)
    {
        Debug.Log("RequestOwnershipServerRpc called");
        // Cambia la propiedad del objeto en el servidor
        GetComponent<NetworkObject>().ChangeOwnership(newOwnerClientId);
    }

    [ClientRpc]
    private void SetKinematicClientRpc(bool isKinematic)
    {
        Debug.Log("SetKinematicClientRpc called");
        // Aplica el cambio de cinemática en todos los clientes
        rb.isKinematic = isKinematic;
    }
}