using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PonPoRespawnPoint : Prop
{
    public static PonPoRespawnPoint thePoint;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            thePoint = this;
        }
    }
    protected override void Restart()
    {
        if (thePoint != this) return;
        PonPo.ponPo.rig.velocity = Vector2.zero;
        PonPo.ponPo.transform.position = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.6f);
    }
}