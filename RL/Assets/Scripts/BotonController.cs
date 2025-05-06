using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BotonController : MonoBehaviour
{
    public Door door;
    public GameObject puerta;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")){
            if (door.open == false)
            {
                door.OpenDoor();
                puerta.GetComponent<Collider>().isTrigger = true;
            }
            else
            {
                door.CloseDoor();
                puerta.GetComponent<Collider>().isTrigger = false;
            }
        }
    }
}
