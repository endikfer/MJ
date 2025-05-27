using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Components;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody), typeof(XRGrabInteractable))]
public class GunSync : NetworkBehaviour
{
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;
    private Transform attachTransform;
    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Quaternion> networkRotation = new NetworkVariable<Quaternion>();
    private NetworkVariable<ulong> currentOwnerId = new NetworkVariable<ulong>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void Start()
    {
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (!IsSpawned) return;

        attachTransform = args.interactorObject.transform;
        RequestOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
        UpdateGrabState(true);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Liberar ownership tanto para host como clientes
        if (IsOwner)
        {
            ReleaseOwnershipServerRpc();
        }
        UpdateGrabState(false);
        attachTransform = null;
    }

    [ServerRpc]
    private void ReleaseOwnershipServerRpc()
    {
        // Asignar ownership al servidor (ID 0)
        NetworkObject.ChangeOwnership(0);
        currentOwnerId.Value = 0;
        Debug.Log("Ownership liberado");
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ulong clientId)
    {
        // Verificar que el objeto no está ya agarrado por otro jugador
        if (currentOwnerId.Value != 0 && currentOwnerId.Value != clientId) return;

        NetworkObject.ChangeOwnership(clientId);
        currentOwnerId.Value = clientId;
    }

    private void UpdateGrabState(bool isGrabbed)
    {
        rb.isKinematic = isGrabbed;
        rb.useGravity = !isGrabbed;

        if (!isGrabbed)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Forzar sincronización de posición al soltar
            if (TryGetComponent<NetworkTransform>(out var netTransform))
            {
                netTransform.Teleport(transform.position, transform.rotation, transform.localScale);
            }
        }
    }

    private void Update()
    {
        if (attachTransform != null && IsOwner)
        {
            transform.position = attachTransform.position;
            transform.rotation = attachTransform.rotation;
            UpdatePositionServerRpc(transform.position, transform.rotation);
        }
        else if (attachTransform != null)
        {
            transform.position = Vector3.Lerp(transform.position, networkPosition.Value, 15f * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation.Value, 15f * Time.deltaTime);
        }
    }

    [ServerRpc]
    private void UpdatePositionServerRpc(Vector3 position, Quaternion rotation)
    {
        networkPosition.Value = position;
        networkRotation.Value = rotation;
    }

    public override void OnNetworkSpawn()
    {
        networkPosition.OnValueChanged += OnPositionChanged;
        networkRotation.OnValueChanged += OnRotationChanged;
    }

    private void OnPositionChanged(Vector3 oldPos, Vector3 newPos)
    {
        if (!IsOwner && attachTransform != null)
        {
            transform.position = newPos;
        }
    }

    private void OnRotationChanged(Quaternion oldRot, Quaternion newRot)
    {
        if (!IsOwner && attachTransform != null)
        {
            transform.rotation = newRot;
        }
    }

    private void FixedUpdate()
    {
        if (IsOwner && attachTransform != null)
        {
            rb.MovePosition(attachTransform.position);
            rb.MoveRotation(attachTransform.rotation);
        }
    }

    private void OnDestroy()
    {
        networkPosition.OnValueChanged -= OnPositionChanged;
        networkRotation.OnValueChanged -= OnRotationChanged;
    }
}