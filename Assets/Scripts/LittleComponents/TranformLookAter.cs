using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("little Components/TranformLookAter")]
public class TranformLookAter : MonoBehaviour
{
    public void LookAt(Vector2 direction)
    {
        transform.rotation = Quaternion.LookRotation(direction, Vector3.forward);
    }
}
