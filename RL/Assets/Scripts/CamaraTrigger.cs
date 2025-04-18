using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraTrigger : MonoBehaviour
{
    public Transform newCameraPosition;
    public CamaraController camaraController;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            camaraController.MoveToPosition(newCameraPosition.position);
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
