using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadWater : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _Floating == null)
        {
            _Floating = StartCoroutine(Floating(PonPo.ponPo.rig));
            PonPo.ponPo.rig.drag = GameSystem.TheMatrix.PonPoSetting.enemyFloatDrag + 2;
            PonPo.ponPo.Die();
        }
        else if (collision.isTrigger == false && collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().TurnIntoGround(GetComponent<BoxCollider2D>().bounds.max.y);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _Floating != null)
        {
            StopCoroutine(_Floating);
            _Floating = null;
            PonPo.ponPo.rig.drag = 0;
        }
    }

    private Coroutine _Floating;
    WaitForFixedUpdate wu = new WaitForFixedUpdate();
    private IEnumerator Floating(Rigidbody2D r)
    {
        while (true)
        {
            yield return wu;
            r.AddForce(Vector2.up * GameSystem.TheMatrix.PonPoSetting.enemyFloatPower);
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
