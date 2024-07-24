using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour
{
    public Road Road;
    public MainGuy MainGuy;
    public WeaponsList weaponsStaticData;
    public AttackFXController fxController;

    private List<Attackable> enemies = new List<Attackable>(20);
    private Dictionary<WeaponType, WeaponDefinition> weapons;
    private Coroutine attackCoroutine = null;
    private WeaponDefinition currentWeapon;

    private float attackStartPosition;
    private float attackEndPosition;
    private float teamDamage = 0.5f;

    void Start() {
        SubscriveEvents();
        fxController.SetEnemies(enemies);
    }

    void Update() {
        if (Road.MovementStarted && enemies.Count > 0) {
            AttackEnemies();
        } else if (enemies.Count == 0) {
            fxController.ClearAttackFXs();
        }
    }

    void OnDestroy() =>
        UnsubscriveEvents();

    public void HandleAttackObject(RoadObjectBase roadObject) {
        if (NotAllInitialized())
            return;

        if (roadObject is Attackable enemy) {
            if (!enemy.CanBeAttacked) 
                return;

            if (IsMainGuyReached(enemy)) {
                RemoveEnemy(enemy);
            } else if (IsInAttackRange(enemy)) {
                if (IsInAttackWidth(
                    position: enemy.transform.position.x,
                    enemy.Renderer.bounds.size.x / 2
                )) {
                    AddEnemy(enemy);
                } else {
                    RemoveEnemy(enemy);
                }
            }
        }
    }
    
    public void Prepare() {
        InitAttackRange();
        PrepareWeapons();
        HandleWeaponChanged(MainGuy.CurrentWeaponType);
    }

    public bool IntersectsTeam(RoadObjectBase roadObject) {
        if (MainGuy == null)
            return false;

        float nearest = roadObject.transform.position.z - roadObject.transform.lossyScale.z / 2;
        if (!PlayerIsReached(nearest)) {
            return false;
        }
        float farthest = roadObject.transform.position.z + roadObject.transform.lossyScale.z / 2;
        if (PlayerIsDrovePast(farthest)) {
            return false;
        }

        float objectHalfWidth = roadObject.transform.lossyScale.x / 2;
        float objectX = roadObject.transform.position.x;
        float teamNearestLeft = TeamNearestLeftPoint();
        float teamNearestRight = TeamNearestRightPoint();

        if (objectX - objectHalfWidth >= teamNearestLeft && objectX - objectHalfWidth <= teamNearestRight
            || objectX + objectHalfWidth >= teamNearestLeft && objectX + objectHalfWidth <= teamNearestRight
        ) {
            return nearest >= TeamFarthestPoint();
        }
        return false;
    }

    private bool NotAllInitialized() =>
        MainGuy == null || MainGuy.Team == null || MainGuy.Team.MostLeftMate == null;
        
    private bool PlayerIsReached(float nearestZPointToPlayer) =>
        nearestZPointToPlayer < MainGuy.transform.position.z + MainGuy.transform.lossyScale.z / 2;

    private bool PlayerIsDrovePast(float farthestZPointToPlayer) =>
        farthestZPointToPlayer < MainGuy.transform.position.z - MainGuy.transform.lossyScale.z / 2;

    private float TeamNearestLeftPoint() =>
        MainGuy.Team.MostLeftMate.transform.position.x
        - MainGuy.Team.MostLeftMate.transform.lossyScale.x / 2;

    private float TeamNearestRightPoint() =>
        MainGuy.Team.MostRightMate.transform.position.x
        + MainGuy.Team.MostRightMate.transform.lossyScale.x / 2;

    private float TeamFarthestPoint() =>
        MainGuy.transform.position.z - MainGuy.transform.lossyScale.z;

    private void SubscriveEvents() =>
        EventManager.StartListening(EventNames.WeaponChanged, EventWeaponChanged);

    private void UnsubscriveEvents() =>
        EventManager.StopListening(EventNames.WeaponChanged, EventWeaponChanged);

    private void EventWeaponChanged(object argument) {
        if (argument is WeaponType weaponType) {
            HandleWeaponChanged(weaponType);
        }
    }

    private void InitAttackRange() {
        float MainGuyZCoord = MainGuy.transform.position.z;
        attackStartPosition = MainGuyZCoord + MainGuy.Team.AttackRange + 1;
        attackEndPosition = MainGuyZCoord + 1;
    }

    private void PrepareWeapons() {
        weapons = new Dictionary<WeaponType, WeaponDefinition>(weaponsStaticData.Items.Length);
        foreach (WeaponDefinition weapon in weaponsStaticData.Items) {
            weapons.Add(weapon.Type, weapon);
        }
    }

    private void HandleWeaponChanged(WeaponType weaponType) {
        currentWeapon = weapons[weaponType];
        InitTeamDamage();
    }

    private void InitTeamDamage() =>
        teamDamage = currentWeapon.Damage * MainGuy.DamageMultiplier
            + MainGuy.Team.MatesCount * currentWeapon.Damage;

    private void AttackEnemies() {
        if (attackCoroutine != null) return;

        attackCoroutine = StartCoroutine(Utils.WaitAndDo(currentWeapon.AttackCooldown, () => {
            for (int i = 0; i < enemies.Count; i++) {
                if (enemies[i].CanBeAttacked) {
                    enemies[i].HP -= teamDamage;
                    if (enemies[i].HP > 0)
                        HandleAttackVisualisation();
                }
                if (enemies[i] == null || enemies[i].HP <= 0) {
                    DoNotAttackIfPickable(enemies[i]);
                    enemies.RemoveAt(i);
                    i--;
                }
            }

            attackCoroutine = null;
        }));
    }

    private void DoNotAttackIfPickable(Attackable enemy) {
        if (enemy != null) {
            if (enemy is Weapon weapon) {
                weapon.DisableBeenAttacked();
            }
        }
    }

    private void HandleAttackVisualisation() {
        if (!fxController.IsPrepared(currentWeapon.Type)) {
            fxController.PrepareAttackFXs(
                weapon: currentWeapon,
                fxSpawner: currentWeapon.FxSpawner,
                leftEdge: MainGuy.Team.MostLeftMate,
                rightEdge: MainGuy.Team.MostRightMate,
                attackStartPosition,
                attackEndPosition
            );
        }
    }

    private bool IsInAttackRange(Attackable enemy) =>
        enemy.transform.position.z - enemy.transform.lossyScale.z <= attackStartPosition;

    private bool IsMainGuyReached(Attackable enemy) =>
        enemy.transform.position.z <= attackEndPosition;

    private bool IsInAttackWidth(float position, float halfWidth) {
        return position >= MainGuy.Team.MostLeftMate.position.x - halfWidth
            && position <= MainGuy.Team.MostRightMate.position.x + halfWidth;
    }

    private void AddEnemy(Attackable enemy) {
        if (enemies.Contains(enemy))
            return;
        enemies.Add(enemy);
    }

    private void RemoveEnemy(Attackable enemy) {
        enemy.VisualiseTakeDamage(false);
        enemies.Remove(enemy);
    }
}
