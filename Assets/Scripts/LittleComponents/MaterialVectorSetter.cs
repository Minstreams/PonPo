using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("little Components/MaterialVectorSetter")]
public class MaterialVectorSetter : MonoBehaviour
{
    public GameObject materialObject;
    private void Awake()
    {
        material = materialObject.GetComponent<Renderer>().material;
    }

    private Material material;
    public string paraName;


    public void SetMaterial(Vector2 direction)
    {
        material.SetVector(paraName, direction);
    }
}
