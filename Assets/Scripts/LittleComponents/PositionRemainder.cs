using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[AddComponentMenu("little Components/PositionRemainder")]
public class PositionRemainder : MonoBehaviour
{
    public float recoilDistance;
    public float smoothness = 0.5f;

    public void Recoil(Vector2 dir)
    {
        transform.position -= (Vector3)(recoilDistance * dir);
    }
    private void Update()
    {
        transform.localPosition *= Mathf.Pow(smoothness, Time.deltaTime);
    }
}
