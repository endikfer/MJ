using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;

[RequireComponent(typeof(Rigidbody), typeof(ClientNetworkTransform))]
public class BulletController : NetworkBehaviour
{
    public float speed = 30f;
    public float lifetime = 3f;

    private Rigidbody rb;
    private bool velocityApplied = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            rb.velocity = transform.forward * speed;
            velocityApplied = true;
            Invoke(nameof(DestroyBullet), lifetime);
        }
        else
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }
    }

    private void FixedUpdate()
    {
        if (IsServer && !velocityApplied)
        {
            rb.velocity = transform.forward * speed;
            velocityApplied = true;
        }
    }

    private void DestroyBullet()
    {
        if (IsServer && IsSpawned)
        {
            NetworkObject.Despawn(true);
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        NetworkObject playerObj = other.GetComponent<NetworkObject>();
        if (playerObj != null && playerObj.OwnerClientId == OwnerClientId)
            return;

        DestroyBullet();
    }
}
