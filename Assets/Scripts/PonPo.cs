using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Player
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PonPo : MonoBehaviour
{
    //Paras & References
    public static PonPo ponPo;
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
    public AudioSource audioReload;
    public AudioSource audioJump;

    public Material shootTorchMaterial;

    private void Awake()
    {
        ponPo = this;
        rig = GetComponent<Rigidbody2D>();
    }



    //Input Convertion
    private bool ILeft { get => Input.GetKey(KeyCode.A); }
    private bool IRight { get => Input.GetKey(KeyCode.D); }
    private bool IJump { get => Input.GetKeyDown(KeyCode.Space); }
    private bool IShootUp { get => Input.GetKeyDown(KeyCode.UpArrow); }
    private bool IShootDown { get => Input.GetKeyDown(KeyCode.DownArrow); }
    private bool IShootLeft { get => Input.GetKeyDown(KeyCode.LeftArrow); }
    private bool IShootRight { get => Input.GetKeyDown(KeyCode.RightArrow); }
    private bool IShootBegin = false;
    private bool IShootEnd { get => !(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)); }







    //Event list
    public static event UnityAction Restart;

    [System.Serializable]
    public class Vec2Event : UnityEvent<Vector2> { }
    public Vec2Event onShoot;
    public UnityEvent onDie;



    //Gun Control=============================================================
    public int ammo = 2;
    private static Vector2 direction;
    public static bool ShootHit(Vector3 position)
    {
        Vector2 vec = position - ponPo.transform.position;
        return vec.magnitude < GameSystem.TheMatrix.PonPoSetting.cannonDistance && Mathf.Abs(Vector2.Angle(vec, direction)) < GameSystem.TheMatrix.PonPoSetting.cannonAngle;
    }


    IEnumerator Shoot()
    {
        while (true)
        {
            ammo = 2;
            float force = 0;

            while (ammo > 0)
            {
                yield return 0;
                if (IShootUp)
                {
                    direction = Vector2.up;
                    force = Setting.gunForceVertical;
                }
                else if (IShootDown)
                {
                    ClearYAxisVolecity();
                    direction = Vector2.down;
                    force = Setting.gunForceVertical;
                }
                else if (IShootLeft)
                {
                    direction = Vector2.left;
                    force = Setting.gunForceHorizontal;
                }
                else if (IShootRight)
                {
                    direction = Vector2.right;
                    force = Setting.gunForceHorizontal;
                }
                else continue;

                //Shoot
                React(-direction * force);
                onShoot?.Invoke(direction);
                ammo--;

                //ammo time
                IShootBegin = true;

                //Reload
                audioReload.Play();
                float ttimer = Setting.reloadTime;
                while (ammo <= 0 && ttimer > 0)
                {
                    yield return 0;
                    ttimer -= Time.deltaTime;
                }
                audioReload.Stop();
            }
        }
    }
    IEnumerator AmmoTime()
    {
        while (true)
        {
            while (true)
            {
                yield return 0;
                if (IShootBegin)
                {
                    IShootBegin = false;
                    break;
                }
            }

            yield return new WaitForSeconds(Setting.ammoTimeDelay);

            if (IShootEnd) continue;

            Time.timeScale = Setting.ammoTimeFactor * Setting.ammoTimeFactor;
            float timer = 1f;
            while (timer > 0)
            {
                yield return 0;
                timer -= Time.deltaTime / Setting.ammoTimeSeconds / Time.timeScale;
                float t = Mathf.Lerp(1f, Setting.ammoTimeFactor, timer);
                Time.timeScale = t * t;


                if (IShootBegin)
                {
                    IShootBegin = false;
                    timer = 1f;
                }
                if (IShootEnd) break;
            }
            yield return 0;
            Time.timeScale = 1.0f;
        }
    }
    private void Start()
    {
        StartCoroutine(Shoot());
        StartCoroutine(AmmoTime());
    }



    //React===================================================================
    public void React(Vector2 force)
    {
        rig.AddForce(force + Vector2.up * Setting.reactPower, ForceMode2D.Impulse);
    }

    public void ClearYAxisVolecity()
    {
        Vector2 v = rig.velocity;
        v.y = 0f;
        rig.velocity = v;
    }

    public void Damage(Vector2 force)
    {
        ammo = 0;
        React(force);
        print("Damage!");
    }

    public void Die()
    {
        print("Die!");
        onDie?.Invoke();
        StartCoroutine(InvokeRestart());
    }

    private IEnumerator InvokeRestart()
    {
        yield return new WaitForSeconds(Setting.ammoTimeDelay);
        Time.timeScale = Setting.ammoTimeFactor * Setting.ammoTimeFactor;
        float timer = 1f;
        while (timer > 0)
        {
            yield return 0;
            timer -= Time.deltaTime / Setting.dieDelayTime / Time.timeScale;
            float t = Mathf.Lerp(1f, Setting.ammoTimeFactor, timer);
            Time.timeScale = t * t;

        }
        yield return 0;
        Time.timeScale = 1.0f;
        ammo = 2;
        Restart?.Invoke();
    }






    //Moving===================================================================
    private bool isGround = false;
    private int groundAttached = 0; //the number of attached grounds
    private float Force { get => isGround ? Setting.groundForce : Setting.airForce; }

    private void FixedUpdate()
    {
        float a = 0f;
        if (ILeft) a -= 1f;
        if (IRight) a += 1f;
        rig.AddForce(Vector2.right * a * Force - (isGround ? rig.velocity * Setting.groundDrag * Setting.groundDrag : Vector2.zero), ForceMode2D.Force);

        Debug.DrawLine(transform.position, transform.position + Vector3.right * a * Force * 0.1f, Color.blue);
        Debug.DrawLine(transform.position, transform.position - (isGround ? (Vector3)rig.velocity * Setting.groundDrag * Setting.groundDrag : Vector3.zero) * 0.1f, Color.gray);
    }
    private void Update()
    {
        if (IJump && isGround)
        {
            ClearYAxisVolecity();
            audioJump.Play();
            React(Vector2.up * Setting.jumpPower);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            if (groundAttached <= 0)
            {
                isGround = true;
            }
            groundAttached++;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            groundAttached--;
            if (groundAttached <= 0)
            {
                isGround = false;
            }
        }
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.1f);
        float angle = Setting.cannonAngle * Mathf.PI / 180f;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * Setting.cannonDistance);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(angle), -Mathf.Sin(angle), 0) * Setting.cannonDistance);
        Gizmos.DrawLine(transform.position + Vector3.right * Setting.cannonDistance, transform.position + new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * Setting.cannonDistance);
        Gizmos.DrawLine(transform.position + Vector3.right * Setting.cannonDistance, transform.position + new Vector3(Mathf.Cos(angle), -Mathf.Sin(angle), 0) * Setting.cannonDistance);

        Gizmos.DrawLine(transform.position, transform.position + new Vector3(-Mathf.Cos(angle), Mathf.Sin(angle), 0) * Setting.cannonDistance);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(-Mathf.Cos(angle), -Mathf.Sin(angle), 0) * Setting.cannonDistance);
        Gizmos.DrawLine(transform.position + Vector3.left * Setting.cannonDistance, transform.position + new Vector3(-Mathf.Cos(angle), Mathf.Sin(angle), 0) * Setting.cannonDistance);
        Gizmos.DrawLine(transform.position + Vector3.left * Setting.cannonDistance, transform.position + new Vector3(-Mathf.Cos(angle), -Mathf.Sin(angle), 0) * Setting.cannonDistance);

        Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * Setting.cannonDistance);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(-Mathf.Sin(angle), Mathf.Cos(angle), 0) * Setting.cannonDistance);
        Gizmos.DrawLine(transform.position + Vector3.up * Setting.cannonDistance, transform.position + new Vector3(Mathf.Sin(angle), Mathf.Cos(angle), 0) * Setting.cannonDistance);
        Gizmos.DrawLine(transform.position + Vector3.up * Setting.cannonDistance, transform.position + new Vector3(-Mathf.Sin(angle), Mathf.Cos(angle), 0) * Setting.cannonDistance);

        Gizmos.DrawLine(transform.position, transform.position + new Vector3(Mathf.Sin(angle), -Mathf.Cos(angle), 0) * Setting.cannonDistance);
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(-Mathf.Sin(angle), -Mathf.Cos(angle), 0) * Setting.cannonDistance);
        Gizmos.DrawLine(transform.position + Vector3.down * Setting.cannonDistance, transform.position + new Vector3(Mathf.Sin(angle), -Mathf.Cos(angle), 0) * Setting.cannonDistance);
        Gizmos.DrawLine(transform.position + Vector3.down * Setting.cannonDistance, transform.position + new Vector3(-Mathf.Sin(angle), -Mathf.Cos(angle), 0) * Setting.cannonDistance);

        Gizmos.color = Color.white;
    }
}
