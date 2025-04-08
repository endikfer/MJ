using Unity.Netcode;
using Unity.Netcode.Components;

public class SyncPlayerTransform : NetworkBehaviour
{
    private void Start()
    {
        if (!IsOwner)
        {
            // Deshabilitar la sincronización de transformaciones para clientes que no tienen autoridad
            GetComponent<NetworkTransform>().enabled = false;
        }
    }
}
