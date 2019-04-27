using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("little Components/WheelRotater")]
public class WheelRotater : MonoBehaviour
{
    public Rigidbody2D relatedRigidbody;
    private Vector2 _referenceVec = Vector2.right;
    Quaternion rot = Quaternion.Euler(0, 0, 90);
    public Vector2 referenceNormal { set { _referenceVec = rot * value; } }
    public Vector3 rotateAxis = Vector3.back;
    public float radius = 0.5f;

    public void SetNegNormal(Vector2 n)
    {
        referenceNormal = -n;
    }

    private void Update()
    {
        Vector2 speed = relatedRigidbody.velocity;
        transform.Rotate(rotateAxis, Time.deltaTime * Mathf.Sign(Vector2.Dot(_referenceVec, speed)) * speed.magnitude * 180f / radius / Mathf.PI);
    }
}
