using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadStone : Prop
{
    bool onAir = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (onAir)
        {
            if (collision.collider.CompareTag("Player"))
            {
                PonPo.ponPo.Die();
            }
            else if (collision.collider.CompareTag("Ground"))
            {
                onAir = false;
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();
        PonPo.ShootAction += OnPonpoShoot;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        PonPo.ShootAction -= OnPonpoShoot;
    }

    private void OnPonpoShoot(Vector2 direction)
    {
        direction = -direction;
        Vector2 vec = transform.position - PonPo.ponPo.transform.position;
        if (vec.magnitude < GameSystem.TheMatrix.PonPoSetting.cannonDistance && Mathf.Abs(Vector2.Angle(vec, direction)) < GameSystem.TheMatrix.PonPoSetting.cannonAngle)
        {
            print("stone hit!");
            GetComponent<Rigidbody2D>().isKinematic = false;
            GetComponent<Rigidbody2D>().AddForce(direction, ForceMode2D.Impulse);
        }
    }

    private void OnDrawGizmos()
    {
        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        if (!circle) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(circle.bounds.center, circle.radius);
        Gizmos.color = Color.white;
    }

    protected override void Restart()
    {
        base.Restart();
        onAir = true;
    }
}
