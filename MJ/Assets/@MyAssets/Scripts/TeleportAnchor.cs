using UnityEngine;
using Unity.Netcode;

public class TeleportAnchor : NetworkBehaviour
{
    [SerializeField] private Transform teleportTarget;
    private bool recentlyTeleported = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsOwner || recentlyTeleported) return;

        NetworkObject netObj = other.GetComponent<NetworkObject>();
        if (netObj != null && netObj.IsOwner)
        {
            recentlyTeleported = true;
            RequestTeleportServerRpc(netObj.OwnerClientId);

            // Evita el bucle con un retraso
            Invoke(nameof(ResetTeleportFlag), 2f);
        }
    }

    private void ResetTeleportFlag()
    {
        recentlyTeleported = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestTeleportServerRpc(ulong clientId)
    {
        // Busca al jugador por ClientId
        NetworkObject playerNetObj = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;
        if (playerNetObj != null)
        {
            // Realiza el movimiento desde el servidor
            TeleportClientRpc(clientId, teleportTarget.position, teleportTarget.rotation.eulerAngles);
        }
    }

    [ClientRpc]
    private void TeleportClientRpc(ulong targetClientId, Vector3 newPosition, Vector3 newRotationEuler, ClientRpcParams rpcParams = default)
    {
        // Solo ejecuta en el cliente dueño
        if (NetworkManager.Singleton.LocalClientId != targetClientId) return;

        Transform playerTransform = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject().transform;

        CharacterController controller = playerTransform.GetComponent<CharacterController>();
        if (controller != null) controller.enabled = false;

        playerTransform.SetPositionAndRotation(newPosition, Quaternion.Euler(newRotationEuler));

        if (controller != null) controller.enabled = true;
    }
}
