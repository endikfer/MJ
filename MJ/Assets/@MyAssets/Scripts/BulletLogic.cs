using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(Rigidbody), typeof(NetworkObject))]
public class BulletLogic : NetworkBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (IsServer)
        {
            rb.velocity = transform.forward * speed;
            Invoke(nameof(DestroySelf), lifetime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        if (IsSpawned)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}