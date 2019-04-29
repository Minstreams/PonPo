using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("little Components/DelayEvent")]
public class DelayEvent : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent e;
    public void InvokeEvent(float seconds)
    {
        Invoke("temp", seconds);
    }

    private void temp()
    {
        e?.Invoke();
    }
}
