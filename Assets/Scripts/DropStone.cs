using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropStone : Prop
{
    private Rigidbody2D stone;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            stone.isKinematic = false;
        }

    }

    protected override void Awake()
    {
        base.Awake();
        stone = GetComponentInChildren<Rigidbody2D>();
    }
    protected override void Restart()
    {
        base.Restart();
        stone.velocity = Vector2.zero;
        stone.angularVelocity = 0;
        stone.isKinematic = true;
        stone.transform.localRotation = Quaternion.identity;
        stone.transform.localPosition = Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (!box) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(box.bounds.center, 2 * box.bounds.extents);
        Gizmos.color = Color.white;
    }
}
