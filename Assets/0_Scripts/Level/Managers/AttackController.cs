using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    [SerializeField] private MainGuy MainGuy;

    [NonSerialized] public List<EnemyBase> enemies = new List<EnemyBase>();
    [NonSerialized] private Cooldown AttackCooldown = new Cooldown(1);
    [SerializeField] private float Damage = 1;

    void Update()
    {
        HandleAttack();
    }

    void FixedUpdate()
    {
        // HandleAttack();
    }

    public void InitTeamDamage()
    {
        Damage = MainGuy.Team.TeamCount + 1;
    }

    public void AddNewEnemy(EnemyBase enemy)
    {
        enemies.Add(enemy);
    }

    public void RemoveEnemy(EnemyBase enemy)
    {
        enemies.Remove(enemy);
    }

    private void HandleAttack()
    {
        if (enemies.Count == 0) return;

        MainGuy.VisualiseAttack(enemies[0].transform.position);

        if (AttackCooldown.IsReady)
        {
            enemies[0].HP -= Damage;
            AttackCooldown.Reset();

            if (enemies[0].HP <= 0)
            {
                enemies.RemoveAt(0);
            }
        }
    }
}
