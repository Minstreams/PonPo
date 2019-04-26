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

    private void Start()
    {
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
    }

    protected override void Restart()
    {
        base.Restart();
        if (destroyable) PonPo.ponPo.onShoot.AddListener(OnPonpoShoot);
        broken = false;
        timer = startTimer;
        ker.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position + (Vector3)(transform.localToWorldMatrix * offset), 0.2f);

        Gizmos.color = Color.white;
    }

}
