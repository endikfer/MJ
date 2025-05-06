using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Door : MonoBehaviour {
    public bool open;
	public float smooth = 1.0f;
	float DoorOpenAngle = 90.0f;
    float DoorCloseAngle = 0.0f;

    private Quaternion targetRotation;

    void Start()
    {
        // Puerta inicialmente cerrada
        targetRotation = Quaternion.Euler(0, DoorCloseAngle, 0);
    }

    // Use this for initialization
    void Update()
    {
        // Transición suave hacia la rotación deseada
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * 5 * smooth);
    }

    public void OpenDoor()
    {
        open = true;
        targetRotation = Quaternion.Euler(0, DoorOpenAngle, 0);
    }

    public void CloseDoor()
    {
        open = false;
        targetRotation = Quaternion.Euler(0, DoorCloseAngle, 0);
    }
}