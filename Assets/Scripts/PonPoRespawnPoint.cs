using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PonPoRespawnPoint : Prop
{
    public static PonPoRespawnPoint thePoint;
    public UnityEngine.Events.UnityEvent onActivate;
    public UnityEngine.Events.UnityEvent onDeActivate;
    public UnityEngine.Events.UnityEvent onReborn;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && thePoint != this)
        {
            if (thePoint != null) thePoint.onDeActivate?.Invoke();
            thePoint = this;
            onActivate?.Invoke();
        }
    }
    protected override void Restart()
    {
        if (thePoint != this) return;
        onReborn?.Invoke();
        PonPo.ponPo.rig.velocity = Vector2.zero;
        PonPo.ponPo.transform.position = transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 0.6f);
    }
}