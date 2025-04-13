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

    // Añadimos NetworkVariables para sincronizar la animación
    public NetworkVariable<float> horizontalVelocity = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> verticalVelocity = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> speed = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isDeadNetwork = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

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

    // Update is called once per frame
    void Update()
    {
        if (!IsOwner || xrCharacterController == null || animator == null)
            return;

        // Si el personaje está muerto, cambiamos el estado de muerte
        if (isDeadNetwork.Value)
        {
            animator.SetBool("IsDead", true);
            return;
        }

        // Usamos los valores sincronizados de las NetworkVariables
        animator.SetFloat("Horizontal", horizontalVelocity.Value);
        animator.SetFloat("Vertical", verticalVelocity.Value);
        animator.SetFloat("speed", speed.Value);
    }

    public void Die()
    {
        isDead = true;
    }
}
