using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PonPo.ponPo.ammo++;
        }
    }

    private void OnDrawGizmos()
    {
        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        if (!circle) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(circle.bounds.center, circle.radius);
        Gizmos.color = Color.white;
    }
}
