using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class BulletController : NetworkBehaviour
{
    public float speed = 30f;
    public float lifetime = 3f;
    private Rigidbody rb;
    private bool velocityApplied = false;
    private ulong shooterId;
    public Collider shooterCollider;

    public void SetShooter(ulong shooterId, Collider shooterCol)
    {
        this.shooterId = shooterId;
        this.shooterCollider = shooterCol;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            if (shooterCollider != null && TryGetComponent<Collider>(out var bulletCollider))
            {
                Physics.IgnoreCollision(bulletCollider, shooterCollider);
            }

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

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        NetworkObject targetObj = other.GetComponent<NetworkObject>();
        if (targetObj != null && targetObj.OwnerClientId == shooterId) return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            var playerLife = other.GetComponent<PlayerLife>();
            if (playerLife != null)
            {
                playerLife.TakeDamage(10f);
            }
        }

        DestroyBullet();
    }

    private void DestroyBullet()
    {
        if (IsServer && IsSpawned)
        {
            NetworkObject.Despawn(true);
            Destroy(gameObject);
        }
    }
}