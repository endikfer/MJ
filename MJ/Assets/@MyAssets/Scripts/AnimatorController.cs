using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class AnimatorController : NetworkBehaviour
{
    private Animator animator;
    private CharacterController xrCharacterController;

    public string xrOriginName = "XR Origin"; // asegúrate de que se llame así

    public bool isDead = false;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return; // solo controla su propio avatar

        animator = GetComponent<Animator>();

        // Intenta buscar el XR Origin del jugador local
        GameObject xrOrigin = GameObject.Find(xrOriginName);
        if (xrOrigin != null)
        {
            xrCharacterController = xrOrigin.GetComponent<CharacterController>();
        }

        if (xrCharacterController == null)
        {
            Debug.LogWarning("CharacterController no encontrado en XR Origin.");
        }
    }

    void Update()
    {
        if (!IsOwner || xrCharacterController == null || animator == null)
            return;

        if (isDead)
        {
            animator.SetBool("IsDead", true);
            return;
        }

        // Velocidad local respecto al avatar
        Vector3 localVelocity = transform.InverseTransformDirection(xrCharacterController.velocity);
        float horizontal = localVelocity.x;
        float vertical = localVelocity.z;

        Debug.Log("Horizontal Velocity: " + horizontal);
        Debug.Log("Vertical Velocity: " + vertical);

        if (Mathf.Abs(horizontal) < 0.1f && Mathf.Abs(vertical) < 0.1f)
        {
            horizontal = 0f;
            vertical = 0f;
        }

        float currentSpeed = new Vector2(horizontal, vertical).magnitude;

        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
        animator.SetFloat("speed", currentSpeed);


        if (currentSpeed < 0.1f) // Si la velocidad es suficientemente baja
        {
            animator.SetBool("IsWalking", false);  // Detiene la animación de caminar
        }
        else
        {
            animator.SetBool("IsWalking", true);   // Inicia la animación de caminar
        }
    }

    public void Die()
    {
        isDead = true;
    }
}
