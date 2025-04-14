using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR;

public class PlayerSpawn : NetworkBehaviour
{
    public Transform xrRig; // La raíz del XR Rig (por ejemplo: XR Origin)
    public Transform SpawnPolice;
    public Transform SpawnContrab;


    void Start()
    {
        if (!IsOwner) return;

        // Buscamos el punto de spawn según el tipo de jugador
        int playerType = FullGameManager.Instance.GetPlayerType(NetworkManager.LocalClientId);

        if (playerType == 0)
        {
            xrRig.transform.position = SpawnContrab.transform.position;
            xrRig.transform.rotation = SpawnContrab.transform.rotation;
        }
        else if(playerType == 1)
        {
            xrRig.transform.position = SpawnPolice.transform.position;
            xrRig.transform.rotation = SpawnPolice.transform.rotation;
        }
    }
}
