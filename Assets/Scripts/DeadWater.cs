using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadWater : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PonPo.ponPo.Die();
        }
        else if (collision.CompareTag("Enemy"))
        {
            Enemy e = collision.GetComponent<Enemy>();
            e.rig.velocity = Vector2.zero;
            e.rig.angularVelocity = 0;
            e.transform.rotation = Quaternion.identity;
            e.tag = "Ground";
            e.TurnIntoGround(new Vector2(e.transform.position.x, GetComponent<BoxCollider2D>().bounds.max.y));
        }
    }

    private void OnDrawGizmos()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        if (!box) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(box.bounds.center, 2 * box.bounds.extents);
        Gizmos.color = Color.white;
    }
}
