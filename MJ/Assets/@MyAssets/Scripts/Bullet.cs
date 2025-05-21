using Unity.Netcode;
using UnityEngine;

public class Bullet : NetworkBehaviour
{
    public float lifeTime = 2f;
    public float damage = 10f; 

    private void Start()
    {
        if (IsServer)
        {
            Destroy(gameObject, lifeTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsServer) return;

        Destroy(gameObject);
    }
}
