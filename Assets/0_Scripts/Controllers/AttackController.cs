using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Road Road;
    public MainGuy MainGuy;
    public float TeamDamage = 0.5f;
    // private Cooldown attackCooldown = new Cooldown(0.1f);
    private float attackCooldown = 0.1f;

    private List<EnemyBase> enemies = new List<EnemyBase>(20);

    private float attackStartPosition;
    private float attackEndPosition;

    private Coroutine attackCoroutine = null;

    void Update() {
        if (Road.MovementStarted && enemies.Count > 0) {
            AttackEnemies();
        }
    }

    public void HandleAttackObject(RoadObjectBase roadObject) {
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

    public void Prepare() {
        InitAttackRange();
        InitTeamDamage();
    }

    private void InitAttackRange() {
        float MainGuyZCoord = MainGuy.transform.position.z;
        attackStartPosition = MainGuyZCoord + MainGuy.Team.AttackRange + 1;
        attackEndPosition = MainGuyZCoord + 1;
    }

    private void InitTeamDamage() {
        TeamDamage = MainGuy.Damage + MainGuy.Team.MatesCount * MainGuy.Team.AmogusDamage;
    }

    private void AttackEnemies() {
        if (attackCoroutine != null) return;

        attackCoroutine = StartCoroutine(Utils.WaitAndDo(attackCooldown, () => {
            for (int i = 0; i < enemies.Count; i++) {
                enemies[i].HP -= TeamDamage;
                enemies[i].VisualiseTakingDamage(true);

                if (enemies[i].HP <= 0) {
                    enemies[i].Kill();
                    enemies.RemoveAt(i);
                    i--;
                }
            }
            
            attackCoroutine = null;
        }));
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
        if (enemies.Contains(enemy)) return;
        enemies.Add(enemy);
    }

    private void RemoveEnemy(EnemyBase enemy) {
        enemy.VisualiseTakingDamage(false);
        enemies.Remove(enemy);
    }

}
