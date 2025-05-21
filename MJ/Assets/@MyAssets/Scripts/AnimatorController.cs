using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class AnimatorController : NetworkBehaviour
{
    private Animator animator;
    private CharacterController xrCharacterController;

    public string xrOriginName = "XR Origin";
    private NetworkAnimator networkAnimator;

    public bool isDead = false;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        animator = GetComponent<Animator>();
        networkAnimator = GetComponent<NetworkAnimator>();

        // Intenta buscar el XR Origin del jugador local
        GameObject xrOrigin = GameObject.Find(xrOriginName);
        if (xrOrigin != null)
        {
            xrCharacterController = xrOrigin.GetComponent<CharacterController>();
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
            DieServerRpc();
        }
    }

    [ServerRpc]
    public void DieServerRpc()
    {
        isDead = true;
        networkAnimator.SetTrigger("Die");
    }
}