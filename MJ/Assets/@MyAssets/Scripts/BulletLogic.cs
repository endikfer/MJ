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

    public void Init()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

        if (IsServer)
        {
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