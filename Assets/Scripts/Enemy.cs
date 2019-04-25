using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    //Paras & References
    private GameSystem.PresentSetting.PonPoSetting _setting;
    private GameSystem.PresentSetting.PonPoSetting Setting
    {
        get
        {
            if (!_setting) _setting = GameSystem.TheMatrix.PonPoSetting;
            return _setting;
        }
    }

    [HideInInspector] public Rigidbody2D rig;
    private bool isAlive = true;

    private void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        PonPo.ponPo.onShoot.AddListener(OnPonpoShoot);
    }

    ///Alive---------------------------------------------------------------------
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isAlive && collision.collider.CompareTag("Player"))
        {
            Vector2 impulse = PonPo.ponPo.transform.position - transform.position;
            float xabs = Mathf.Abs(impulse.x);
            float yabs = Mathf.Abs(impulse.y);
            float force = 0;
            if (yabs > xabs)
            {
                impulse.x = 0;
                impulse.y /= yabs;
                force = Setting.damageForceVertical;
            }
            else
            {
                impulse.x /= xabs;
                impulse.y = 0;
                force = Setting.damageForceHorizontal;
            }
            PonPo.ponPo.Damage(impulse * force);
        }
    }

    //Acting Pattern

    //React
    public UnityEngine.Events.UnityAction DieAction;
    public void React(Vector2 direction)
    {
        rig.AddForce(direction + Vector2.up * Setting.enemyReactPower, ForceMode2D.Impulse);
    }
    private void OnPonpoShoot(Vector2 direction)
    {
        Vector2 vec = transform.position - PonPo.ponPo.transform.position;
        if (vec.magnitude < Setting.cannonDistance && Mathf.Abs(Vector2.Angle(vec, direction)) < Setting.cannonAngle)
        {
            if (isAlive)
            {
                Die();
                React(direction * Setting.enemyHitPowerAlive);
                gameObject.layer = LayerMask.NameToLayer("Enemy Dead");
                transform.Translate(Vector3.forward * 0.1f);
            }
            else
            {
                React(direction * Setting.enemyHitPowerDead);
            }
            print("Hit!");
        }
    }

    public void TurnIntoGround(Vector2 pos)
    {
        if (isAlive) Die();
        rig.isKinematic = true;
        gameObject.layer = LayerMask.NameToLayer("Ground");
        //TODO:pos,animation
    }

    private void Die()
    {
        DieAction?.Invoke();
        isAlive = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.color = Color.white;
    }
}
