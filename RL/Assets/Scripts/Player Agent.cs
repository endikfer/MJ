using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PlayerAgent : Agent
{
    public bool useVectorObs;

    private int currentLevel = 1;
    private int successStreak = 0;
    private int totalLevels = 4;
    public int successesRequired = 10;

    public Transform levelStartPosition;
    public Transform[] levelTargets;

    public GameObject[] puertas;

    public Rigidbody rb;

    public CamaraController camaraController;
    public Transform[] newCameraPositions;

    private int currentDoorIndex = 0;

    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVectorObs)
        {
            sensor.AddObservation(StepCount / (float)MaxStep);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        if (StepCount >= MaxStep && successStreak < successesRequired)
        {
            RegisterFailure();
        }

        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var actionMove = actionBuffers.DiscreteActions[0];
        var actionRotate = actionBuffers.DiscreteActions[1];
        var actionJump = actionBuffers.DiscreteActions[2];
        

        switch (actionMove)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
        }

        switch (actionRotate)
        {
            case 1:
                transform.Rotate(Vector3.up * -1f);
                break;
            case 2:
                transform.Rotate(Vector3.up * 1f);
                break;
        }

        switch (actionJump)
        {
            case 1:
                if (IsGrounded())
                {
                    var rb = GetComponent<Rigidbody>();
                    rb.AddForce(Vector3.up * 100f, ForceMode.Impulse);
                }
                break;
        }
        transform.position += dirToGo * Time.deltaTime;
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 0.3f);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
        if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[1] = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[1] = 2;
        }
        if (Input.GetKey(KeyCode.Space))
        {
            discreteActionsOut[2] = 1;
        }
    }

    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        transform.localPosition = levelStartPosition.localPosition;

        currentDoorIndex = 0;
        

        if (camaraController != null && newCameraPositions.Length >= currentLevel - 1)
        {
            camaraController.MoveToPosition(newCameraPositions[0].position);
        }

        foreach (var door in GameObject.FindGameObjectsWithTag("Puerta"))
        {
            if (door.name == "puerta")
            {
                Collider doorCol = door.GetComponent<Collider>();
                if (doorCol != null)
                {
                    doorCol.isTrigger = true;
                }
            }

            if(door.name == "Puerta" && door.GetComponent<Collider>().isTrigger == true)
            {
                door.GetComponent<Collider>().isTrigger = false;
                
            }
            if(door.name == "Door_3_Yellow" && door.GetComponentInChildren<Door>().open == true)
            {
                door.GetComponentInChildren<Door>().open = false;
                door.GetComponentInChildren<Door>().CloseDoor();
            }
        }
    }

    // Puedes usar este método cuando detectes que el agente completó el objetivo correctamente
    public void RegisterSuccess()
    {
        successStreak++;

        if (successStreak >= successesRequired)
        {
            if (currentLevel < totalLevels)
            {
                currentLevel++;
                successStreak = 0;
                Debug.Log("¡Avanzas al nivel " + currentLevel + "!");
            }
            else
            {
                Debug.Log("¡Has completado todos los niveles!");
            }
        }

        EndEpisode();
    }

    // Si el agente falla o no termina en MaxStep, llamas a esto
    public void RegisterFailure()
    {
        successStreak = 0;
        currentLevel = 1;
        EndEpisode();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Puerta"))
        {
            int doorsPerLevel = currentLevel; // Nivel 2 = 2 puertas, nivel 3 = 3 puertas...

            currentDoorIndex++;

            if (currentDoorIndex < doorsPerLevel)
            {
                // Cambiar cámara a la siguiente posición
                int camIndex = Mathf.Min(currentDoorIndex, newCameraPositions.Length - 1);
                if (camaraController != null)
                {
                    camaraController.MoveToPosition(newCameraPositions[camIndex].position);
                }
            }
            else
            {
                // Última puerta -> éxito
                RegisterSuccess();
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        //Debug.Log("Saliendo de: " + other.name);
        if (other.CompareTag("Puerta"))
        {
            // Solo cerrar puertas si ya avanzó al menos al nivel 2
            if (currentLevel > 1)
            {
                // Solo cerramos la puerta si corresponde al nivel anterior
                if (currentDoorIndex == 1)
                {
                    other.GetComponent<Collider>().isTrigger = false;
                }
                else if (currentDoorIndex > 1)
                {
                    Debug.Log("Nuemero puertas: " + currentDoorIndex);
                    Debug.Log("Nuemero puerta: " + puertas[currentDoorIndex - 2]);
                    puertas[currentDoorIndex - 2].GetComponentInChildren<Door>().CloseDoor();
                }
            }
        }
    }


}
