using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrRigReferences : MonoBehaviour
{
    public static VrRigReferences Singleton;

    public Transform root;
    public Transform head;
    public Transform rightHand;
    public Transform leftHand;

    private void Awake()
    {
        Singleton = this;
    }
}
