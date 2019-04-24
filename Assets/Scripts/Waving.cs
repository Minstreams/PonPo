using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waving : Prop
{
    public float range;
    public float speed;
    public float startTimer;

    private float timer = 0;
    private void Update()
    {
        timer += Time.deltaTime * speed * Mathf.PI / 2f;
        float angle = range * Mathf.Sin(timer);
        transform.localRotation = Quaternion.Euler(0, 0, angle);
    }

    protected override void Awake()
    {
        base.Awake();
        timer = startTimer * Mathf.PI / 2f;
    }

    protected override void Restart()
    {
        base.Restart();
        timer = startTimer * Mathf.PI / 2f;
    }
}
