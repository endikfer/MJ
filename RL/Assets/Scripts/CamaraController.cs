using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraController : MonoBehaviour
{
    public void MoveToPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }
}
