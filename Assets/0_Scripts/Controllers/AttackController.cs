using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Road Road;
    public MainGuy MainGuy;
    public WeaponsList weaponsStaticData;

    private List<EnemyBase> enemies = new List<EnemyBase>(20);
    private Dictionary<WeaponType, WeaponDefinition> weapons;
    private Coroutine attackCoroutine = null;
    private WeaponDefinition currentWeapon;
    private float attackCooldown = 0.1f;
    private float attackStartPosition;
    private float attackEndPosition;
    private float teamDamage = 0.5f;

    void Start() {
        SubscriveEvents();
    }
    void Update() {
        if (Road.MovementStarted && enemies.Count > 0) {
            AttackEnemies();
        }
    }
    void OnDestroy() {
        UnsubscriveEvents();
    }

    public void HandleAttackObject(RoadObjectBase roadObject) {
        if (roadObject is EnemyBase enemy) {
            if (IsMainGuyReached(enemy)) {
                RemoveEnemy(enemy);
            } else if (IsInAttackRange(enemy)) {
                if (IsInAttackWidth(position: enemy.transform.position.x)) {
                    AddEnemy(enemy);
                } else {
                    RemoveEnemy(enemy);
                }
            }
        }
    }
    public void Prepare() {
        PrepareWeapons();
        InitAttackRange();
        InitTeamDamage();
    }

    private void SubscriveEvents() {
        //
    }
    private void UnsubscriveEvents() {
        //
    }
    // private void HandleWeaponChanged(WeaponType weaponType) {
    //     currentWeapon = weapons[weaponType];
    //     InitTeamDamage();
    // }
    private void PrepareWeapons() {
        weapons = new Dictionary<WeaponType, WeaponDefinition>(weaponsStaticData.Items.Count());
        foreach (WeaponDefinition weapon in weaponsStaticData.Items) {
            weapons.Add(weapon.Type, weapon);
        }
        currentWeapon = weaponsStaticData.Items[1]; //todo
    }
    private void InitAttackRange() {
        float MainGuyZCoord = MainGuy.transform.position.z;
        attackStartPosition = MainGuyZCoord + MainGuy.Team.AttackRange + 1;
        attackEndPosition = MainGuyZCoord + 1;
    }
    private void InitTeamDamage() {
        // teamDamage = MainGuy.Damage + MainGuy.Team.MatesCount * MainGuy.Team.AmogusDamage;
        teamDamage = currentWeapon.Damage + MainGuy.Team.MatesCount * MainGuy.Team.AmogusDamage;
    }
    private void AttackEnemies() {
        if (attackCoroutine != null) return;

        attackCoroutine = StartCoroutine(Utils.WaitAndDo(attackCooldown, () => {
            for (int i = 0; i < enemies.Count; i++) {
                enemies[i].HP -= teamDamage;
                enemies[i].VisualiseTakeDamage(true);

                if (enemies[i] == null || enemies[i].HP <= 0) {
                    enemies.RemoveAt(i);
                    i--;
                }
            }
            
            attackCoroutine = null;
        }));
    }
    private bool IsInAttackRange(EnemyBase enemy) {
        return enemy.transform.position.z <= attackStartPosition;
    }
    private bool IsMainGuyReached(EnemyBase enemy) {
        return enemy.transform.position.z <= attackEndPosition;
    }
    private bool IsInAttackWidth(float position) {
        return position >= MainGuy.Team.MostLeftMate.position.x
            && position <= MainGuy.Team.MostRightMate.position.x;
    }
    private void AddEnemy(EnemyBase enemy) {
        if (enemies.Contains(enemy)) return;
        enemies.Add(enemy);
    }
    private void RemoveEnemy(EnemyBase enemy) {
        enemy.VisualiseTakeDamage(false);
        enemies.Remove(enemy);
    }
}
