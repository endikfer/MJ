using Unity.Netcode;
using UnityEngine;
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
        if (!IsOwner)
        {
            RequestOwnershipServerRpc(NetworkManager.Singleton.LocalClientId);
        }

        attachTransform = args.interactorObject.transform;
        UpdateGrabState(true);
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        UpdateGrabState(false);
        attachTransform = null;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestOwnershipServerRpc(ulong clientId)
    {
        NetworkObject.ChangeOwnership(clientId);
        currentOwnerId.Value = clientId;
    }

    private void UpdateGrabState(bool isGrabbed)
    {
        if (IsOwner)
        {
            rb.isKinematic = isGrabbed;
            rb.useGravity = !isGrabbed;

            if (!isGrabbed)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }

    private void Update()
    {
        if (attachTransform != null)
        {
            if (IsOwner)
            {
                // Actualización precisa para el dueño
                transform.position = attachTransform.position;
                transform.rotation = attachTransform.rotation;
                UpdatePositionServerRpc(transform.position, transform.rotation);
            }
            else
            {
                // Interpolación suave para clientes remotos
                transform.position = Vector3.Lerp(transform.position, networkPosition.Value, 15f * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(transform.rotation, networkRotation.Value, 15f * Time.deltaTime);
            }
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
}