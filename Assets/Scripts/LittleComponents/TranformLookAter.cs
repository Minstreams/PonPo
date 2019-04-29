using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("little Components/TranformLookAter")]
public class TranformLookAter : MonoBehaviour
{
    public Vector3 dir = Vector3.up;
    public Vector3 axis = Vector3.forward;
    [ContextMenu("Look")]
    public void LookManually()
    {
        transform.rotation = Quaternion.LookRotation(dir, axis);
    }
    public void LookAt(Vector2 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction, Vector3.forward);
    }
}
