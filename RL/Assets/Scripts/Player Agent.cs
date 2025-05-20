using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Unity.VisualScripting;

public class PlayerAgent : Agent
{
    public bool useVectorObs;

    private bool canJump = false;

    private bool hasArrive = false;
    private bool hasToArrive = false;

    private int currentLevel = 1;
    private int successStreak = 0;
    private int totalLevels = 4;
    public int successesRequired = 10;
    public bool button1 = false;
    public bool button2 = false;
    public bool button3 = false;
    public bool button4 = false;

    public Transform levelStartPosition;
    public Transform[] levelTargets;

    public Transform destino;

    public GameObject[] puertas;

    public GameObject[] pared;

    public GameObject boton1;
    public GameObject boton2;
    public GameObject boton3;
    public GameObject boton4;

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

        sensor.AddObservation(transform.position);
        sensor.AddObservation(boton1.transform.position);
        sensor.AddObservation(boton2.transform.position);
        sensor.AddObservation(boton3.transform.position);
        sensor.AddObservation(boton4.transform.position);
        sensor.AddObservation(Vector3.Distance(transform.position, levelTargets[currentDoorIndex].position ));
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
                dirToGo = transform.forward * 4f;
                break;
            case 2:
                dirToGo = transform.forward * -4f;
                break;
        }

        switch (actionRotate)
        {
            case 1:
                transform.Rotate(Vector3.up * -4f);
                break;
            case 2:
                transform.Rotate(Vector3.up * 4f);
                break;
        }

        switch (actionJump)
        {
            case 1:
                if (canJump)
                {
                    rb = GetComponent<Rigidbody>();
                    rb.AddForce(Vector3.up * 80f, ForceMode.Impulse);
                }
                break;
        }
        Vector3 targetPosition = rb.position + dirToGo * Time.deltaTime;
        rb.MovePosition(targetPosition);

        PenalizeByDistanceToGoal();

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
        if (hasToArrive == true && hasArrive != true)
        {
            currentLevel = 1;
            successStreak = 0;
            hasToArrive = false;
        }
        else
        {

        }

        hasArrive = false;

        if (currentLevel == 4)
        {
            hasToArrive = true;
        }


        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        button1 = false;
        button2 = false;
        button3 = false;
        button4 = false;

        pared[0].SetActive(true);
        pared[1].SetActive(true);
        pared[2].SetActive(true);

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
                Debug.LogError("¡Avanzas al nivel " + currentLevel + "!");
                
            }
            else
            {
                Debug.Log("¡Has completado todos los niveles!");
                AddReward(400.0f);
                hasArrive = true;
            }
        }

        EndEpisode();
    }

    // Si el agente falla o no termina en MaxStep, llamas a esto
    public void RegisterFailure()
    {
        AddReward(-50.0f);
        successStreak = 0;
        currentLevel = 1;
        hasArrive = false;
        EndEpisode();
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Puerta"))
        {
            Vector3 doorPosition = other.transform.position;
            Vector3 agentPosition = transform.position;

            float direction = agentPosition.x - doorPosition.x;

            // Avance correcto si el agente se está moviendo hacia una menor X (hacia la siguiente puerta/nivel)
            bool isCorrectDirection = direction < 0;

            int doorsPerLevel = currentLevel;

            if (isCorrectDirection)
            {
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





                if (currentDoorIndex == 3)
                {
                    Debug.Log("Puerta 3 atravesada.");
                }
                else if (currentDoorIndex == 4)
                {
                    Debug.Log("Puerta 4 atravesada.");
                }
                else if (currentDoorIndex == 2)
                {
                    Debug.Log("Puerta 2 atravesada.");
                }
            }


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
                    puertas[currentDoorIndex - 2].GetComponentInChildren<Door>().CloseDoor();
                }
            }
        }

        if (other.gameObject.CompareTag("Buton1"))
        {
            pared[0].SetActive(!pared[0].activeSelf);

            if (button1 == false)
            {
                button1 = true;
                AddReward(20.0f);
                Debug.Log("Boton 1 pulsado.");
            }
        }
        if (other.gameObject.CompareTag("Buton2"))
        {
            pared[1].SetActive(!pared[1].activeSelf);

            if (button2 == false)
            {
                button2 = true;
                AddReward(20.0f);
                Debug.Log("Boton 2 pulsado.");
            }
        }
        if (other.gameObject.CompareTag("Buton3") || other.gameObject.CompareTag("Buton4"))
        {
            pared[2].SetActive(!pared[2].activeSelf);

            if (button3 == false)
            {
                button3 = true;
                AddReward(20.0f);
                Debug.Log("Boton 3 pulsado.");
            }
            else if (button4 == false)
            {
                button4 = true;
                AddReward(20.0f);
                Debug.Log("Boton 4 pulsado.");
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Jump"))
        {
            canJump = true;
        }
        
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Jump"))
        {
            canJump = false;
        }
    }

    private void PenalizeByDistanceToGoal()
    {
        if (levelTargets.Length >= currentLevel)
        {
            float distance = Vector3.Distance(transform.position, destino.position);
            float maxExpectedDistance = 45.68f;
            float normalizedPenalty = Mathf.Clamp01(distance / maxExpectedDistance);

            AddReward(-normalizedPenalty * 0.5f);
        }
    }
}
