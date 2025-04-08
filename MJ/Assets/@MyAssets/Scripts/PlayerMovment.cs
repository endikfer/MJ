using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerMovment : NetworkBehaviour
{
    public GameObject xrOrigin;
    public GameObject leftHandController;
    public GameObject rightHandController;

    void Start()
    {
        if (!IsOwner)
        {
            // Desactivás locomotion, input y otros sistemas para los jugadores remotos
            xrOrigin.GetComponent<LocomotionSystem>().enabled = false;
            leftHandController.SetActive(false);
            rightHandController.SetActive(false);
            // También podrías desactivar los ray interactors, teleport providers, etc.
        }
    }
}
