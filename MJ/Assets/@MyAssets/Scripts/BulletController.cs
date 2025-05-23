using UnityEngine;
using Unity.Netcode;

public class BulletController : NetworkBehaviour
{
    public float damage = 10f;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        // Obtener el NetworkObject del objeto impactado
        NetworkObject hitObject = other.GetComponentInParent<NetworkObject>();

        // Si es nulo o somos dueños de este objeto (evitar auto-disparo)
        if (hitObject == null || hitObject.OwnerClientId == OwnerClientId) return;

        // Verificar si es un jugador
        if (other.CompareTag("Player"))
        {
            /*var playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }*/
        }

        // Destruir la bala siempre
        GetComponent<NetworkObject>().Despawn();
        Destroy(gameObject);
    }
}