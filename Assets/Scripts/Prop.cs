using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Prop : MonoBehaviour
{
    protected virtual void Awake()
    {
        PonPo.Restart += Restart;
    }
    protected virtual void OnDestroy()
    {
        PonPo.Restart -= Restart;
    }

    protected virtual void Restart()
    {

    }
}
