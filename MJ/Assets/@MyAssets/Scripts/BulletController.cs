using UnityEngine;
using Unity.Netcode;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;

[RequireComponent(typeof(Rigidbody), typeof(ClientNetworkTransform))]
public class BulletController : NetworkBehaviour
{
    public float speed = 30f;
    public float lifetime = 3f;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            rb.velocity = transform.forward * speed;
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

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            PlayerLife playerLife = other.GetComponentInParent<PlayerLife>();
            if (playerLife != null)
            {
                Debug.Log($"[SERVER] Bullet hit player {playerLife.OwnerClientId}");
                playerLife.TakeDamage(10f);
            }
        }

        DestroyBullet();
    }

    private void DestroyBullet()
    {
        if (IsServer && IsSpawned)
        {
            NetworkObject.Despawn();
            Destroy(gameObject);
        }
    }
}
