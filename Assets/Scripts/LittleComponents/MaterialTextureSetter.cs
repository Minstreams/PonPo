using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("little Components/MaterialTextureSetter")]
public class MaterialTextureSetter : MonoBehaviour
{
    public Material material;
    public string paraName;
    public Texture[] tex;

    public void SetMaterial(int index)
    {
        material.SetTexture(paraName, tex[index]);
    }
}
