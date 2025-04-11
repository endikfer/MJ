using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{

    public Transform root;
    public Transform head;
    public Transform rightHand;
    public Transform leftHand;

    //public Renderer[] meshToDisable;

    /*public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        if (IsOwner)
        {
            foreach (var item in meshToDisable)
            {
                item.enabled = false;
            }
        }
    }*/



    // Update is called once per frame
    void Update()
    {
        Debug.Log("Actualizando");
        if (IsOwner)
        {
            Debug.Log("Soy owner, por lo que me voy a mocer a " + VrRigReferences.Singleton.root.position);
            root.position = VrRigReferences.Singleton.root.position;
            root.rotation = VrRigReferences.Singleton.root.rotation;

            head.position = VrRigReferences.Singleton.head.position;
            head.rotation = VrRigReferences.Singleton.head.rotation;

            rightHand.position = VrRigReferences.Singleton.rightHand.position;
            rightHand.rotation = VrRigReferences.Singleton.rightHand.rotation;

            leftHand.position = VrRigReferences.Singleton.leftHand.position;
            leftHand.rotation = VrRigReferences.Singleton.leftHand.rotation;
        }
    }
}
