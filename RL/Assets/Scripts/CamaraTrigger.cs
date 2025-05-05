using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using UnityEngine;

public class CamaraTrigger : MonoBehaviour
{
    public Transform newCameraPosition;
    public CamaraController camaraController;
    public PlayerAgent agent;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            camaraController.MoveToPosition(newCameraPosition.position);
            agent.RegisterSuccess();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GetComponent<Collider>().isTrigger = false;
        }
    }
}
