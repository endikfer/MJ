using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class PlayerAgent : Agent
{
    public bool useVectorObs;

    private readonly float[] maxExpectedDistances = new float[] { 7.88f, 20.55f, 31.42f, 40.68f };

    private bool canJump = false;
    private bool isOffButton = false;
    private bool hasJumped = false;

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
                    rb.AddForce(Vector3.up * 45f, ForceMode.Impulse);
                    hasJumped = true;
                }
                break;
        }
        Vector3 targetPosition = rb.position + dirToGo * Time.deltaTime;
        rb.MovePosition(targetPosition);

        if (isOffButton && hasJumped)
        {
            isOffButton = false;  // Resetear la variable
        }

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
        AddReward(5.0f);

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
                AddReward(20.0f);
            }
        }

        EndEpisode();
    }

    // Si el agente falla o no termina en MaxStep, llamas a esto
    public void RegisterFailure()
    {
        AddReward(-1.0f);
        successStreak = 0;
        currentLevel = 1;
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
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Jump") || collision.gameObject.CompareTag("Buton"))
        {
            canJump = true;
        }

        if (!collision.gameObject.CompareTag("Buton") && hasJumped)
        {
            isOffButton = true;  // Aterrizó en algo que no es un botón
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Jump") || collision.gameObject.CompareTag("Buton"))
        {
            canJump = false;
        }
    }

    private void PenalizeByDistanceToGoal()
    {
        if (levelTargets.Length >= currentLevel)
        {
            float distance = Vector3.Distance(transform.position, levelTargets[currentLevel - 1].position);
            float maxExpectedDistance = maxExpectedDistances[currentLevel - 1];
            float normalizedPenalty = Mathf.Clamp01(distance / maxExpectedDistance);


            //debug log. 

            Debug.Log(-normalizedPenalty * 0.005f);
            AddReward(-normalizedPenalty * 0.005f);
        }
    }
}
