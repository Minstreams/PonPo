using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("little Components/MaterialColorSetter")]
public class MaterialColorSetter : MonoBehaviour
{
    public GameObject materialObject;
    private Material material;
    private Renderer rder;
    private void Awake()
    {
        rder = materialObject.GetComponent<Renderer>();
        material = rder.material;
    }

    public string paraName;

    [ColorUsage(true, true)]
    public Color[] color;


    public void SetMaterial(int index)
    {
        material.SetColor(paraName, color[index]);
        //DynamicGI.SetEmissive(materialObject.GetComponent<Renderer>(), color[index]);
        //rder.UpdateGIMaterials();
    }
}
