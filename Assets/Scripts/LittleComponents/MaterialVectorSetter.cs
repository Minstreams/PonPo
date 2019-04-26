using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("little Components/MaterialVectorSetter")]
public class MaterialVectorSetter : MonoBehaviour
{
    public Material material;
    public string paraName;

    public void SetMaterial(Vector2 direction)
    {
        material.SetVector(paraName, direction);
    }
}
