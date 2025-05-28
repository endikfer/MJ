using UnityEngine;
using Unity.Netcode;

[RequireComponent(typeof(BulletSync), typeof(Rigidbody))]
public class BulletController : NetworkBehaviour
{
    public float speed = 30f;
    public float lifetime = 3f;

    private void Start()
    {
        if (IsServer)
        {
            // Solo el servidor aplica física
            GetComponent<Rigidbody>().velocity = transform.forward * speed;
            Invoke(nameof(DestroyBullet), lifetime);
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
        DestroyBullet();
    }
}