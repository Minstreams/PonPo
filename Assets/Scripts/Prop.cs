using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Prop : MonoBehaviour
{
    protected virtual void Awake()
    {
        GameSystem.TheMatrix.Restart += Restart;
    }
    protected virtual void OnDestroy()
    {
        GameSystem.TheMatrix.Restart -= Restart;
    }

    protected virtual void Restart()
    {

    }
}
