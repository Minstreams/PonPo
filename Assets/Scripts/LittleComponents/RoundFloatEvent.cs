using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

[AddComponentMenu("little Components/RoundFloatEvent")]
public class RoundFloatEvent : MonoBehaviour
{
    public FloatEvent[] e;
    private int i = 0;

    public void InvokeEvent(float input)
    {
        e[i]?.Invoke(input);
        i++;
        if (i >= e.Length) i = 0;
    }
}
