using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("little Components/RoundEvent")]
public class RoundEvent : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent[] e;
    private int i = 0;

    public void InvokeEvent()
    {
        e[i]?.Invoke();
        i++;
        if (i >= e.Length) i = 0;
    }
}
