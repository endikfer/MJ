using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class AnimatorController : NetworkBehaviour
{
    private Animator animator;
    private CharacterController xrCharacterController;

    public string xrOriginName = "XR Origin"; // asegúrate de que se llame así
    private NetworkAnimator networkAnimator;

    public bool isDead = false;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return; // solo controla su propio avatar

        animator = GetComponent<Animator>();
        networkAnimator = GetComponent<NetworkAnimator>();

        // VERIFICACIONES EXTRA
        if (animator == null)
        {
            Debug.LogError("Animator no encontrado en el jugador.");
        }
        else if (animator.runtimeAnimatorController == null)
        {
            Debug.LogError("Animator Controller no asignado al Animator.");
        }

        if (networkAnimator == null)
        {
            Debug.LogWarning("NetworkAnimator no encontrado en el jugador.");
        }

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

        float currentSpeed = new Vector2(horizontal, vertical).magnitude;

        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
        animator.SetFloat("speed", currentSpeed);
    }

    public void Die()
    {
        if (IsOwner)
        {
            // Llama al RPC para sincronizar el estado de "muerte"
            DieServerRpc();
        }
    }

    [ServerRpc]
    public void DieServerRpc()
    {
        isDead = true;

        // Sincroniza el estado en todos los clientes
        networkAnimator.SetTrigger("Die");
    }
}
