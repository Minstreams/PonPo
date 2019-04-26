using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Prop
{
    private bool picked = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!picked && collision.CompareTag("Player"))
        {
            PonPo.ponPo.Ammo++;
            print("ammo:" + PonPo.ponPo.Ammo);
            picked = true;
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

    protected override void Restart()
    {
        base.Restart();
        picked = false;
    }
}
