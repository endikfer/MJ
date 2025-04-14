using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR;

public class PlayerSpawn : NetworkBehaviour
{
    public Transform SpawnPolice;
    public Transform SpawnContrab;

    /*public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        // Espera un pequeño tiempo para asegurarse de que XR Rig está listo
        StartCoroutine(DelayedSpawn());
    }

    private System.Collections.IEnumerator DelayedSpawn()
    {
        yield return new WaitForSeconds(0.1f); // Ajusta si es necesario

        // Asegura que tienes acceso al XR Rig local
        Transform xrRig = VrRigReferences.Singleton?.root;
        if (xrRig == null)
        {
            Debug.LogError("No se encontró el XR Rig del jugador local.");
            yield break;
        }

        int playerType = FullGameManager.Instance.GetPlayerType(NetworkManager.LocalClientId);


        if (playerType == 0)
        {
            xrRig.position = SpawnContrab.position;
            xrRig.rotation = SpawnContrab.rotation;
        }
        else if (playerType == 1)
        {
            xrRig.position = SpawnPolice.position;
            xrRig.rotation = SpawnPolice.rotation;
        }
    }*/
}
