using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystem;

public class DeadStone : Prop
{
    bool onAir = true;
    public FloatEvent onLanding;

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
        onLanding?.Invoke(collision.relativeVelocity.magnitude);
    }

    protected override void Start()
    {
        base.Start();
        PonPo.ponPo.onShoot.AddListener(OnPonpoShoot);
    }

    private void OnPonpoShoot(Vector2 direction)
    {
        if (PonPo.ShootHit(transform.position))
        {
            print("stone hit!");
            float force = Mathf.Abs(direction.x) > Mathf.Abs(direction.y) ? GameSystem.TheMatrix.PonPoSetting.gunForceHorizontal : GameSystem.TheMatrix.PonPoSetting.gunForceVertical;
            GetComponent<Rigidbody2D>().isKinematic = false;
            GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
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
