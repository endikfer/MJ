using UnityEngine;
using Unity.Netcode;
using UnityEngine.XR.Interaction.Toolkit;
using Unity.XR.CoreUtils;

public class PlayerXRSetup : NetworkBehaviour
{
    public XROrigin xrOrigin;

    void Start()
    {
        if (IsOwner)
        {
            xrOrigin.gameObject.SetActive(true);
        }
        else
        {
            xrOrigin.gameObject.SetActive(false);
        }
    }
}
