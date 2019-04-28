using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunLifter : MonoBehaviour
{
    private float oneMinusResetFactor = 1;
    [Range(0, 1)]
    public float resetFactorSmooth;

    [Range(0, 1)]
    public float smoothness;
    private Vector3 euler = new Vector3(-90, 0, -180);
    private float targetAngle = 0;
    private bool right = true;

    public void OnShoot(Vector2 direction)
    {
        oneMinusResetFactor = 1;
        euler.y = 0;
        transform.localEulerAngles = euler;
        if (direction.x > 0.5f)
        {
            //right
            targetAngle = 0;
            right = true;
        }
        else if (direction.x < -0.5f)
        {
            //left
            targetAngle = 0;
            right = false;
        }
        else
        {
            //up or down
            targetAngle = (direction.y > 0.5f == right) ? -90 : 90;
        }

    }

    private void Update()
    {
        oneMinusResetFactor *= Mathf.Pow(resetFactorSmooth, Time.deltaTime);
        float factor = (1 - oneMinusResetFactor);
        euler.y += factor * factor * (1 - Mathf.Pow(smoothness, Time.deltaTime)) * (targetAngle - euler.y);
        transform.localEulerAngles = euler;
    }
}
