using UnityEngine;
using Unity.Netcode;

public class BulletLogic : NetworkBehaviour
{
    public float speed = 20f;
    public float lifetime = 3f;

    private void Start()
    {
        GetComponent<Rigidbody>().velocity = transform.forward * speed;

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
