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
            // Eliminar Rigidbody en clientes para evitar conflictos
            Destroy(rb);
            rb = null;
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
            // Movimiento en clientes usando datos de red
            if (rb == null)
            {
                transform.position = networkPosition.Value;
                transform.forward = networkVelocity.Value.normalized;
            }
        }
    }
}