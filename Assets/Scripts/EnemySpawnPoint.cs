using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnPoint : Prop
{
    public int enemyNum = 1;
    public float coolDownTime = 5.0f;
    public GameObject enemyPrifab;
    public bool infinite = false;

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
            if (infinite) g.GetComponent<Enemy>().onDie.AddListener(OnEnemyDie);
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
        Gizmos.color = Color.white;
    }
}
