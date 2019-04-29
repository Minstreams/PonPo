using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Prop : MonoBehaviour
{
    public UnityEngine.Events.UnityEvent onRestart;
    public UnityEngine.Events.UnityEvent onStart;
    protected virtual void Awake()
    {
        PonPo.Restart += Restart;
    }
    protected virtual void Start()
    {
        onStart?.Invoke();
    }
    protected virtual void OnDestroy()
    {
        PonPo.Restart -= Restart;
    }

    protected virtual void Restart()
    {
        onRestart?.Invoke();
    }
}
