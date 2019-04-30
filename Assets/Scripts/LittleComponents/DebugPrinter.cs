using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("little Components/DebugPrinter")]
public class DebugPrinter : MonoBehaviour
{
    public void PrintFloat(float o)
    {
        print(o);
    }
    public void Print(object o)
    {
        print(o);
    }
}
