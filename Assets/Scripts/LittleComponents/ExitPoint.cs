using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("little Components/ExitPoint")]
public class ExitPoint : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent onExit;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            onExit?.Invoke();
        }
    }
}
