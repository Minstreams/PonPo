using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    private void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        PonPo.ponPo.onShoot.AddListener(OnPonpoShoot);
        patrolOriginX = transform.position.x;
        if (patrolMode) patrolRoutine = StartCoroutine(Patrol());
    }



    ///Basic=================================================================

    //events
    public UnityEvent onDie;
    public UnityEvent onSink;
    public UnityEvent onTurnIntoGround;


    //paras
    private bool isAlive = true;


    //functions
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
        else if (isAlive && collision.collider.CompareTag("Enemy"))
        {
            forcingFlip = true;
        }
    }

    //Acting Pattern

    //React
    public void React(Vector2 force)
    {
        rig.AddForce(force + Vector2.up * Setting.enemyReactPower, ForceMode2D.Impulse);
    }
    private void Die()
    {
        onDie?.Invoke();
        isAlive = false;
        if (chaseRoutine != null) StopCoroutine(chaseRoutine);
        if (patrolRoutine != null) StopCoroutine(patrolRoutine);
    }

    private void OnPonpoShoot(Vector2 direction)
    {
        if (PonPo.ShootHit(transform.position))
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
        }
    }
    public void TurnIntoGround(float surface)
    {
        tag = "Ground";
        gameObject.layer = LayerMask.NameToLayer("Ground");

        if (isAlive) Die();
        onSink?.Invoke();

        StartCoroutine(_TurnIntoGround(surface));
    }
    WaitForFixedUpdate wu = new WaitForFixedUpdate();
    private IEnumerator _TurnIntoGround(float surface)
    {
        rig.drag = Setting.enemyFloatDrag;

        //flying first
        print("flying first");
        while (transform.position.y >= surface)
        {
            yield return wu;
        }

        //sinking second
        print("sinking second");
        while (transform.position.y < surface)
        {
            yield return wu;
            rig.AddForce(Vector2.up * Setting.enemyFloatPower, ForceMode2D.Force);
        }

        //flying again
        print("flying again");
        while (transform.position.y >= surface)
        {
            yield return wu;
        }

        //sinking again
        print("sinking again");
        while (transform.position.y < surface)
        {
            yield return wu;
            rig.AddForce(Vector2.up * Setting.enemyFloatPower, ForceMode2D.Force);
        }

        //flying again
        print("flying again");
        while (transform.position.y >= surface)
        {
            yield return wu;
        }

        //sinking again
        print("sinking again");
        while (transform.position.y < surface)
        {
            yield return wu;
            rig.AddForce(Vector2.up * Setting.enemyFloatPower, ForceMode2D.Force);
        }

        onTurnIntoGround?.Invoke();

        Vector3 vec3 = transform.position;
        vec3.y = surface;
        transform.position = vec3;

        rig.velocity = Vector2.zero;
        rig.angularVelocity = 0;
        transform.rotation = Quaternion.identity;
        rig.isKinematic = true;
    }






    ///Chasing=================================================================

    //events
    public UnityEvent onStartChasing;
    public UnityEvent onEndChasing;


    //paras
    public bool chaseMode = false;
    public float chaseSpeed = 3f;
    private bool isChasing = false;
    private Coroutine chaseRoutine;


    //functions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (chaseMode && collision.CompareTag("Player"))
        {
            chaseRoutine = StartCoroutine(Chase());
            isChasing = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (chaseMode && collision.CompareTag("Player"))
        {
            StopCoroutine(chaseRoutine);
            isChasing = false;
        }
    }
    private IEnumerator Chase()
    {
        while (true)
        {
            yield return 0;
            Vector2 v = rig.velocity;
            v.x = Mathf.Sign(PonPo.ponPo.transform.position.x - transform.position.x) * chaseSpeed;
            rig.velocity = v;
        }
    }






    ///Patrol=================================================================
    [HideInInspector]
    public bool patrolMode = false;
    public float patrolSpeed = 2f;
    [HideInInspector]
    public Vector2 patrolRange = new Vector2(-5f, 5f);
    private float patrolOriginX = 0;
    private Coroutine patrolRoutine;
    private bool forcingFlip = false;

    private IEnumerator Patrol()
    {
        while (true)
        {
            //To Left
            while (transform.position.x > patrolOriginX + patrolRange.x)
            {
                yield return 0;
                if (isChasing) continue;
                if (forcingFlip)
                {
                    forcingFlip = false;
                    break;
                }
                Vector2 v = rig.velocity;
                v.x = -patrolSpeed;
                rig.velocity = v;
            }

            //To Right
            while (transform.position.x < patrolOriginX + patrolRange.y)
            {
                yield return 0;
                if (isChasing) continue;
                if (forcingFlip)
                {
                    forcingFlip = false;
                    break;
                }
                Vector2 v = rig.velocity;
                v.x = patrolSpeed;
                rig.velocity = v;
            }
        }
    }



    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);

        if (chaseMode)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f);
            CircleCollider2D cc = GetComponent<CircleCollider2D>();
            Gizmos.DrawWireSphere(transform.position, cc.radius);
        }

        Gizmos.color = Color.white;
    }
}
