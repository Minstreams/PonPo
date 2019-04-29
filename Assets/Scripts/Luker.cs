using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luker : Prop
{
    public float interval = 3.0f;
    public bool destroyable = false;
    public float startTimer = 3.0f;
    public Vector2 offset;

    public GameObject ker;  //the part thar cause damage

    private bool broken = false;
    private float timer = 0;
    private GameObject dz;

    public UnityEngine.Events.UnityEvent onBroken;
    public UnityEngine.Events.UnityEvent onActivate;
    public UnityEngine.Events.UnityEvent onDeActivate;


    protected override void Start()
    {
        base.Start();
        dz = GetComponentInChildren<DeadZone>().gameObject;
        if (destroyable) PonPo.ponPo.onShoot.AddListener(OnPonpoShoot);
        timer = startTimer;
    }

    private void Update()
    {
        if (broken) return;
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            if (dz.activeSelf) onDeActivate?.Invoke();
            else onActivate?.Invoke();
            dz.SetActive(!dz.activeSelf);
            timer = interval;
        }
    }
    private void OnPonpoShoot(Vector2 direction)
    {
        if (PonPo.ShootHit(transform.position + (Vector3)(transform.localToWorldMatrix * offset)))
        {
            Broke();
        }
    }

    private void Broke()
    {
        broken = true;
        PonPo.ponPo.onShoot.RemoveListener(OnPonpoShoot);
        ker.SetActive(false);
        onBroken?.Invoke();
    }

    protected override void Restart()
    {
        base.Restart();
        if (destroyable) PonPo.ponPo.onShoot.AddListener(OnPonpoShoot);
        broken = false;
        timer = startTimer;
        ker.SetActive(true);
        onActivate?.Invoke();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + (Vector3)(transform.localToWorldMatrix * offset), 0.2f);

        Gizmos.color = Color.white;
    }

}
