using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotating : Prop
{
    public float speed = 30;
    private void Update()
    {
        transform.Rotate(Vector3.forward, speed * Time.deltaTime);
    }

    protected override void Restart()
    {
        base.Restart();
        transform.rotation = Quaternion.identity;
    }
}
