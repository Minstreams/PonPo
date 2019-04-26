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

    private bool isGround = false;
    private int groundAttached = 0; //the number of attached grounds
    private float Force { get => isGround ? Setting.groundForce : Setting.airForce; }


    //Input Convertion
    private bool Left { get => Input.GetKey(KeyCode.A); }
    private bool Right { get => Input.GetKey(KeyCode.D); }
    private bool Jump { get => Input.GetKeyDown(KeyCode.Space); }
    private bool ShootUp { get => Input.GetKeyDown(KeyCode.UpArrow); }
    private bool ShootDown { get => Input.GetKeyDown(KeyCode.DownArrow); }
    private bool ShootLeft { get => Input.GetKeyDown(KeyCode.LeftArrow); }
    private bool ShootRight { get => Input.GetKeyDown(KeyCode.RightArrow); }
    private bool shootBegin = false;
    private bool ShootEnd { get => !(Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow)); }


    //Event list
    [System.Serializable]
    public class ShootEvent : UnityEvent<Vector2> { }
    public ShootEvent onShoot;


    //Gun Control
    public int ammo = 2;

    IEnumerator Shoot()
    {
        while (true)
        {
            ammo = 2;
            Vector2 direction;
            float force = 0;

            while (ammo > 0)
            {
                yield return 0;
                if (ShootUp)
                {
                    direction = Vector2.up;
                    force = Setting.gunForceVertical;
                }
                else if (ShootDown)
                {
                    ClearYAxisVolecity();
                    direction = Vector2.down;
                    force = Setting.gunForceVertical;
                }
                else if (ShootLeft)
                {
                    direction = Vector2.left;
                    force = Setting.gunForceHorizontal;
                }
                else if (ShootRight)
                {
                    direction = Vector2.right;
                    force = Setting.gunForceHorizontal;
                }
                else continue;

                //Shoot
                React(-direction * force);
                onShoot.Invoke(direction);
                ammo--;

                shootTorchMaterial.SetVector("_direction", -direction);

                //ammo time
                shootBegin = true;

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
                if (shootBegin)
                {
                    shootBegin = false;
                    break;
                }
            }

            yield return new WaitForSeconds(Setting.ammoTimeDelay);

            if (ShootEnd) continue;

            Time.timeScale = Setting.ammoTimeFactor * Setting.ammoTimeFactor;
            float timer = 1f;
            while (timer > 0)
            {
                yield return 0;
                timer -= Time.deltaTime / Setting.ammoTimeSeconds / Time.timeScale;
                float t = Mathf.Lerp(1f, Setting.ammoTimeFactor, timer);
                Time.timeScale = t * t;


                if (shootBegin)
                {
                    shootBegin = false;
                    timer = 1f;
                }
                if (ShootEnd) break;
            }
            yield return 0;
            Time.timeScale = 1.0f;
        }
    }


    //React
    public void React(Vector2 direction)
    {
        rig.AddForce(direction + Vector2.up * Setting.reactPower, ForceMode2D.Impulse);
    }

    public void ClearYAxisVolecity()
    {
        Vector2 v = rig.velocity;
        v.y = 0f;
        rig.velocity = v;
    }

    public void Damage(Vector2 direction)
    {
        ammo = 0;
        React(direction);
        print("Damage!");
    }

    public void Die()
    {
        ammo = 2;
        print("Die!");
        GameSystem.TheMatrix.SendGameMessage(GameSystem.GameMessage.Restart);
    }

    //Life
    private void Start()
    {
        StartCoroutine(Shoot());
        StartCoroutine(AmmoTime());
    }

    private void FixedUpdate()
    {
        float a = 0f;
        if (Left) a -= 1f;
        if (Right) a += 1f;
        rig.AddForce(Vector2.right * a * Force - (isGround ? rig.velocity * Setting.groundDrag * Setting.groundDrag : Vector2.zero), ForceMode2D.Force);

        Debug.DrawLine(transform.position, transform.position + Vector3.right * a * Force * 0.1f, Color.blue);
        Debug.DrawLine(transform.position, transform.position - (isGround ? (Vector3)rig.velocity * Setting.groundDrag * Setting.groundDrag : Vector3.zero) * 0.1f, Color.gray);
    }

    private void Update()
    {
        if (Jump && isGround)
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
