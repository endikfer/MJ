using Unity.Netcode;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerMovement : NetworkBehaviour
{
    private ActionBasedContinuousMoveProvider moveProvider;
    private ActionBasedContinuousTurnProvider turnProvider;

    private void Start()
    {
        // Obtener los componentes de movimiento y giro
        moveProvider = GetComponent<ActionBasedContinuousMoveProvider>();
        turnProvider = GetComponent<ActionBasedContinuousTurnProvider>();

        // Desactivar el control de movimiento para clientes que no tienen autoridad
        if (!IsOwner)
        {
            // Desactivar los componentes de movimiento y giro para los jugadores que no tienen autoridad
            moveProvider.enabled = false;
            turnProvider.enabled = false;
        }
        else
        {
            // Habilitar los componentes de movimiento y giro para el jugador que tiene autoridad
            moveProvider.enabled = true;
            turnProvider.enabled = true;
        }
    }
}
