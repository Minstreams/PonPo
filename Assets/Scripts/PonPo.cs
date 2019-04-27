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
    private bool isAlive = true;
    public MaterialTextureSetter eye;

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
    [System.Serializable]
    public class IntEvent : UnityEvent<int> { }

    public Vec2Event onShoot;
    public IntEvent onLosingAmmo;
    public IntEvent onAmmoChange;
    public UnityEvent onReload;
    public UnityEvent onDie;
    public UnityEvent onJump;
    public Vec2Event onLandingGround;
    public UnityEvent onOffGround;
    public UnityEvent onDamage;



    //Gun Control=============================================================
    private int _ammo = 2;
    public int Ammo { get => _ammo; set { if (value < _ammo) onLosingAmmo?.Invoke(_ammo - value); onAmmoChange?.Invoke(value); _ammo = value; } }
    private static Vector2 direction;
    public static bool ShootHit(Vector3 position)
    {
        Vector2 vec = position - ponPo.transform.position;
        return vec.magnitude < GameSystem.TheMatrix.PonPoSetting.cannonDistance && Mathf.Abs(Vector2.Angle(vec, direction)) < GameSystem.TheMatrix.PonPoSetting.cannonAngle;
    }
    private float reloadTimer = 0;

    IEnumerator Shoot()
    {
        while (true)
        {
            Ammo = 2;
            float force = 0;

            while (Ammo > 0)
            {
                yield return 0;
                if (!isAlive) continue;
                if (IShootUp)
                {
                    direction = Vector2.up;
                    force = Setting.gunForceVertical;
                    eye.SetMaterial(1);
                }
                else if (IShootDown)
                {
                    ClearYAxisVolecity();
                    direction = Vector2.down;
                    force = Setting.gunForceVertical;
                    eye.SetMaterial(3);
                }
                else if (IShootLeft)
                {
                    direction = Vector2.left;
                    force = Setting.gunForceHorizontal;
                    eye.SetMaterial(2);
                }
                else if (IShootRight)
                {
                    direction = Vector2.right;
                    force = Setting.gunForceHorizontal;
                    eye.SetMaterial(0);
                }
                else continue;

                //Shoot
                Ammo--;
                React(-direction * force);
                onShoot?.Invoke(direction);

                //ammo time
                IShootBegin = true;
            }

            //Reload
            onReload?.Invoke();
            reloadTimer = Setting.reloadTime;
            while (Ammo <= 0 && reloadTimer > 0)
            {
                yield return 0;
                reloadTimer -= Time.deltaTime;
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
        Ammo = 0;
        React(force);
        reloadTimer = Setting.reloadTime;
        onDamage?.Invoke();
    }

    public void Die()
    {
        if (!isAlive) return;
        isAlive = false;
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
        Ammo = 2;
        Restart?.Invoke();
        isAlive = true;
    }






    //Moving===================================================================
    private bool _isGround = false;
    private bool IsGround { get => _isGround; set { _isGround = value; if (!value) onOffGround?.Invoke(); } }
    private int groundAttached = 0; //the number of attached grounds
    private float Force { get => IsGround ? Setting.groundForce : Setting.airForce; }
    private Vector2 jumpVec = Vector2.down;

    private void FixedUpdate()
    {
        float a = 0f;
        if (ILeft)
        {
            a -= 1f;
            eye.SetMaterial(2);
        }
        if (Input.GetKeyDown(KeyCode.W)) eye.SetMaterial(1);
        if (Input.GetKeyDown(KeyCode.S)) eye.SetMaterial(3);
        if (IRight)
        {
            a += 1f;
            eye.SetMaterial(0);
        }
        rig.AddForce(Vector2.right * a * Force - (IsGround ? rig.velocity * Setting.groundDrag * Setting.groundDrag : Vector2.zero), ForceMode2D.Force);

        Debug.DrawLine(transform.position, transform.position + Vector3.right * a * Force * 0.1f, Color.blue);
        Debug.DrawLine(transform.position, transform.position - (IsGround ? (Vector3)rig.velocity * Setting.groundDrag * Setting.groundDrag : Vector3.zero) * 0.1f, Color.gray);
    }
    private void Update()
    {
        if (IJump && IsGround)
        {
            ClearYAxisVolecity();
            onJump?.Invoke();
            React(jumpVec.normalized * Setting.jumpPower);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            if (groundAttached <= 0)
            {
                IsGround = true;
                print("on");
            }
            groundAttached++;
            foreach (ContactPoint2D v in collision.contacts)
            {
                if (v.normal.y > jumpVec.y) jumpVec = v.normal + Vector2.up * Setting.jumpClimbUpRate;
            }
            onLandingGround?.Invoke(jumpVec);
        }
    }
    ContactPoint2D[] contacts = new ContactPoint2D[32];
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            groundAttached--;
            if (groundAttached <= 0)
            {
                IsGround = false;
                print("off");
            }

            int ccount = rig.GetContacts(contacts);
            jumpVec = Vector2.down;
            for (int i = 0; i < ccount; i++)
            {
                if (contacts[i].normal.y > jumpVec.y) jumpVec = contacts[i].normal + Vector2.up * Setting.jumpClimbUpRate;
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
