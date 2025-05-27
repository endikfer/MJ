using Unity.Netcode;
using UnityEngine;

public class ClientNetworkAnimator : NetworkBehaviour
{
    private Animator animator;

    [SerializeField]
    private string[] floatParameters = new string[] { "Horizontal", "Vertical", "speed" };

    [SerializeField]
    private string[] boolParameters = new string[] { "IsDead" };

    public override void OnNetworkSpawn()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (!IsOwner || animator == null)
            return;

        // Enviar parámetros de animación al servidor
        foreach (var param in floatParameters)
        {
            float value = animator.GetFloat(param);
            SendFloatParamToServerRpc(param, value);
        }

        foreach (var param in boolParameters)
        {
            bool value = animator.GetBool(param);
            SendBoolParamToServerRpc(param, value);
        }
    }

    [ServerRpc]
    private void SendFloatParamToServerRpc(string paramName, float value)
    {
        UpdateFloatParamClientRpc(paramName, value);
    }

    [ServerRpc]
    private void SendBoolParamToServerRpc(string paramName, bool value)
    {
        UpdateBoolParamClientRpc(paramName, value);
    }

    [ClientRpc]
    private void UpdateFloatParamClientRpc(string paramName, float value)
    {
        if (IsOwner) return;
        animator.SetFloat(paramName, value);
    }

    [ClientRpc]
    private void UpdateBoolParamClientRpc(string paramName, bool value)
    {
        if (IsOwner) return;
        animator.SetBool(paramName, value);
    }

    // Si usas triggers (como "Die"), necesitas métodos específicos
    [ServerRpc]
    public void TriggerServerRpc(string triggerName)
    {
        TriggerClientRpc(triggerName);
    }

    [ClientRpc]
    private void TriggerClientRpc(string triggerName)
    {
        if (IsOwner) return;
        animator.SetTrigger(triggerName);
    }
}
