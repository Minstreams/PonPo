using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : Prop
{
    public int enemyNum = 1;
    public float coolDownTime = 5.0f;
    public GameObject enemyPrifab;
    public bool infinite = false;

    public bool patrolMode = false;
    public Vector2 patrolRange = new Vector2(-5f, 5f);

    private List<GameObject> enemies = new List<GameObject>();
    private float timer = 0;
    private int count = 0;

    private void Update()
    {
        if (count >= enemyNum) return;
        timer -= Time.deltaTime;
        if (timer < 0)
        {
            GameObject g = GameObject.Instantiate(enemyPrifab, transform);
            enemies.Add(g);
            Enemy e = g.GetComponent<Enemy>();
            e.patrolMode = patrolMode;
            e.patrolRange = patrolRange;
            if (infinite) e.onDie.AddListener(OnEnemyDie);
            count++;
            timer = coolDownTime;
        }
    }

    private void OnEnemyDie()
    {
        count--;
    }
    protected override void Restart()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            Destroy(enemies[i]);
        }
        enemies.Clear();
        count = 0;
        timer = 0;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.6f);

        if (enemyPrifab)
        {
            Enemy e = enemyPrifab.GetComponent<Enemy>();
            if (e.chaseMode)
            {
                Gizmos.color = new Color(1f, 0.5f, 0f);
                CircleCollider2D cc = e.GetComponent<CircleCollider2D>();
                Gizmos.DrawWireSphere(transform.position, cc.radius);
            }
            if (patrolMode)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawRay(transform.position + new Vector3(patrolRange.x, -15f, 0f), Vector3.up * 30f);
                Gizmos.DrawRay(transform.position + new Vector3(patrolRange.y, -15f, 0f), Vector3.up * 30f);
            }
        }
        Gizmos.color = Color.white;
    }
}
