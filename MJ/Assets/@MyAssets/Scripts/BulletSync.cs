using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletSync : NetworkBehaviour
{
    private Rigidbody rb;
    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>();
    private NetworkVariable<Vector3> networkVelocity = new NetworkVariable<Vector3>();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            rb.isKinematic = false;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        else
        {
            rb.isKinematic = true;
            transform.position = networkPosition.Value;
        }
    }

    private void FixedUpdate()
    {
        if (IsServer)
        {
            networkPosition.Value = rb.position;
            networkVelocity.Value = rb.velocity;
        }
        else
        {
            // Movimiento en clientes
            transform.position = networkPosition.Value;
            transform.forward = networkVelocity.Value.normalized;
        }
    }
}