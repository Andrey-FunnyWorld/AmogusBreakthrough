using System;
using System.Collections.Generic;
using UnityEngine;

public class AttackHandler : MonoBehaviour
{
    public Road Road;
    public MainGuy MainGuy;
    public float TeamDamage = 0.5f; //todo calc it somehow
    private Cooldown attackCooldown = new Cooldown(0.1f);

    private Dictionary<float, List<EnemyBase>> allEnemies;
    private List<EnemyBase> enemiesBeingAttacked = new List<EnemyBase>();

    private float attackStartPosition;
    private float attackEndPosition;

    void Update() {
        if (Road.IsRunning && enemiesBeingAttacked.Count > 0) {
            AttackEnemies();
        }
    }

    public void HandleObjectShouldBeAttacked(RoadObjectBase roadObject) {
        if (roadObject is EnemyBase enemy) {
            if (IsReachedMainGuy(enemy)) {
                RemoveEnemy(enemy);
            } else if (IsInAttackRange(enemy)) {
                if (IsInAttackZoneWidth(position: enemy.transform.position.x)) {
                    AddEnemy(enemy);
                } else {
                    RemoveEnemy(enemy);
                }
            }
        }
    }

    public void InitAttackRange(List<float> tracksCoords) {
        float MainGuyZCoord = MainGuy.transform.position.z;
        attackStartPosition = MainGuyZCoord + MainGuy.Team.AttackRange + 1;
        attackEndPosition = MainGuyZCoord + 1;
        InitEnemiesList(tracksCoords);
    }

    public bool CheckDestroyObject(RoadObjectBase obj) {
        return obj is EnemyBase && obj.transform.position.z <= attackEndPosition;
    }

    private void AttackEnemies() {
        if (attackCooldown.IsReady) {
            bool shouldReinitEnemiesToAttack = false;

            foreach (EnemyBase enemy in enemiesBeingAttacked) {
                enemy.HP -= TeamDamage;

                if (enemy.HP <= 0) {
                    shouldReinitEnemiesToAttack = true;
                }
            }

            if (shouldReinitEnemiesToAttack) {
                enemiesBeingAttacked.Clear();
                RemoveDeadEnemies();
                PrepareEnemiesBeingAttacked();
            }

            attackCooldown.Reset();
        }
    }

    private void InitEnemiesList(List<float> tracksCoords) {
        allEnemies = new Dictionary<float, List<EnemyBase>>();
        foreach (float trackCoord in tracksCoords) {
            allEnemies.Add(trackCoord, new List<EnemyBase>());
        }
    }

    private bool IsInAttackRange(EnemyBase enemy) =>
        enemy.transform.position.z <= attackStartPosition;

    private bool IsReachedMainGuy(EnemyBase enemy) =>
        enemy.transform.position.z <= attackEndPosition;

    private bool IsInAttackZoneWidth(float position) {
        return position >= MainGuy.Team.MostLeftMate.position.x
            && position <= MainGuy.Team.MostRightMate.position.x;
    }

    private void AddEnemy(EnemyBase enemy) {
        if (allEnemies[enemy.transform.position.x].Contains(enemy)) return;

        allEnemies[enemy.transform.position.x].Add(enemy);
        allEnemies[enemy.transform.position.x].Sort(CompareEnemiesZCoords);

        PrepareEnemiesBeingAttacked();
    }

    private void RemoveEnemy(EnemyBase enemy) {
        allEnemies[enemy.transform.position.x].Remove(enemy);
        enemiesBeingAttacked.Remove(enemy);
    }

    private void PrepareEnemiesBeingAttacked() {
        enemiesBeingAttacked.Clear();
        foreach(float track in allEnemies.Keys) {
            if (allEnemies[track].Count > 0) {
                enemiesBeingAttacked.Add(allEnemies[track][0]);
            }
        }
    }

    private void RemoveDeadEnemies() {
        foreach(float track in allEnemies.Keys) {
            allEnemies[track].RemoveAll(x => x == null || x.HP <= 0);
        }
    }

    private int CompareEnemiesZCoords(EnemyBase enemy1, EnemyBase enemy2) =>
        enemy1.transform.position.z.CompareTo(enemy2.transform.position.z);

    
}
