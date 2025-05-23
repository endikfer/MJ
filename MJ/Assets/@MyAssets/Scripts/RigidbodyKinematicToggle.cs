using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyKinematicToggle : NetworkBehaviour
{
    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;
    private NetworkVariable<bool> isKinematic = new NetworkVariable<bool>();

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();

        // Configuración inicial importante
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    public override void OnNetworkSpawn()
    {
        isKinematic.OnValueChanged += OnKinematicChanged;
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

        // Solicitar ownership primero
        RequestOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);

        // Configuración local inmediata
        rb.isKinematic = true;
        rb.useGravity = false;

        // Sincronizar con el servidor
        SetKinematicServerRpc(true);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (!IsOwner) return;

        // Configuración local inmediata
        rb.isKinematic = false;
        rb.useGravity = true;

        // Sincronizar con el servidor
        SetKinematicServerRpc(false);
    }

    [ServerRpc]
    private void RequestOwnershipServerRpc(ulong clientId)
    {
        NetworkObject.ChangeOwnership(clientId);
    }

    [ServerRpc]
    private void SetKinematicServerRpc(bool kinematic)
    {
        isKinematic.Value = kinematic;
    }

    private void OnKinematicChanged(bool previous, bool current)
    {
        // Aplicar a todos los clientes
        rb.isKinematic = current;
        rb.useGravity = !current;

        if (!current)
        {
            // Resetear velocidades al soltar
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}