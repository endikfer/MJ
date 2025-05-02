using UnityEngine;
using Unity.Netcode;

public class BulletLogic : NetworkBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f;

    private Rigidbody rb;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

        Invoke(nameof(DestroySelf), lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        DestroySelf();
    }

    private void DestroySelf()
    {
        if (IsSpawned)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}