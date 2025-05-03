using UnityEngine;
using Unity.Netcode;

public class BulletLogic : NetworkBehaviour
{
    public float speed = 100f;
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
            Destroy(gameObject, lifetime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            Destroy(gameObject);
        }
    }
}