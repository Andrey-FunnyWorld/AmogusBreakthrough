using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour {
    public Road Road;
    public MainGuy MainGuy;
    public WeaponsList weaponsStaticData;
    public AttackFXController fxController;
    public GameObject AttackZonePlane;

    List<Attackable> enemies = new List<Attackable>(20);
    Dictionary<WeaponType, WeaponDefinition> weapons;
    Coroutine attackCoroutine = null;
    WeaponDefinition currentWeapon;
    // PerkType primaryPerk;
    // PerkType secondaryPerk;

    float attackStartPosition;
    float attackEndPosition;
    float teamDamage = 0.5f;

    bool extraBossDamage;
    bool extraAttackWidth;

    void Start() {
        SubscriveEvents();
        fxController.SetEnemies(enemies);
    }

    void Update() {
        if (Road.MovementStarted && enemies.Count > 0) {
            AttackEnemies();
        } else if (!Road.MovementStarted) {
            enemies.Clear();
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
                        GetRenderer(enemy).bounds.size.x / 2
                    )
                ) {
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
        if (!PlayerIsReached(nearest)) 
            return false;
        float farthest = roadObject.transform.position.z + roadObject.transform.lossyScale.z / 2;
        if (PlayerIsDrovePast(farthest))
            return false;
        
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

    public void ApplyPerk(PerkType perk) {
        if (perk == PerkType.BossDamage)
            extraBossDamage = true;
        else if (perk == PerkType.AttackZoneVisibility)
            PrepareAttackZoneVisibility();
        else if (perk == PerkType.ExtraAttackWidth)
            extraAttackWidth = true;
    }

    void PrepareAttackZoneVisibility() {
        var teamSize = MainGuy.Team.GetTeamSize();
        AttackZonePlane.transform.localPosition = new Vector3(0, 0, teamSize.z / 2);
        AttackZonePlane.transform.localScale = new Vector3(teamSize.x, 0.01f, teamSize.z);
        AttackZonePlane.transform.parent.gameObject.SetActive(true);
    }

    bool NotAllInitialized() =>
        MainGuy == null || MainGuy.Team == null || MainGuy.Team.MostLeftMate == null;

    bool PlayerIsReached(float nearestZPointToPlayer) =>
        nearestZPointToPlayer < MainGuy.transform.position.z + MainGuy.transform.lossyScale.z / 2;

    bool PlayerIsDrovePast(float farthestZPointToPlayer) =>
        farthestZPointToPlayer < MainGuy.transform.position.z - MainGuy.transform.lossyScale.z / 2;

    float TeamNearestLeftPoint() =>
        MainGuy.Team.MostLeftMate.transform.position.x
        - MainGuy.Team.MostLeftMate.transform.lossyScale.x / 2;

    float TeamNearestRightPoint() =>
        MainGuy.Team.MostRightMate.transform.position.x
        + MainGuy.Team.MostRightMate.transform.lossyScale.x / 2;

    float TeamFarthestPoint() =>
        MainGuy.transform.position.z - MainGuy.transform.lossyScale.z;

    Renderer GetRenderer(RoadObjectBase enemy) {      
        Renderer r = enemy.GetComponent<Renderer>();
        if (r != null) 
            return r;

        var weapon = enemy.GetComponent<Weapon>();
        if (weapon != null)
            return weapon.BoxRenderer;

        var attackable = enemy.GetComponent<Attackable>();
        if (attackable != null)
            return attackable.Renderer;

        return null;
    }

    void EventWeaponChanged(object argument) {
        if (argument is WeaponType weaponType) 
            HandleWeaponChanged(weaponType);
    }

    void InitAttackRange() {
        float MainGuyZCoord = MainGuy.transform.position.z;
        attackStartPosition = MainGuyZCoord + MainGuy.Team.AttackRange + 1;
        attackEndPosition = MainGuyZCoord + 1;
    }

    void PrepareWeapons() {
        weapons = new Dictionary<WeaponType, WeaponDefinition>(weaponsStaticData.Items.Length);
        foreach (WeaponDefinition weapon in weaponsStaticData.Items) 
            weapons.Add(weapon.Type, weapon);
    }

    void HandleWeaponChanged(WeaponType weaponType) {
        currentWeapon = weapons[weaponType];
        InitTeamDamage();
    }

    void InitTeamDamage() =>
        teamDamage = currentWeapon.Damage * MainGuy.DamageMultiplier
            + MainGuy.Team.MatesCount * currentWeapon.Damage;

    void AttackEnemies() {
        if (attackCoroutine != null) return;

        attackCoroutine = StartCoroutine(Utils.WaitAndDo(currentWeapon.AttackCooldown, () => {
            for (int i = 0; i < enemies.Count; i++) {
                enemies[i].HP -= GetDamage(enemies[i]);
                if (enemies[i].HP > 0)
                    HandleAttackVisualisation();

                if (enemies[i] == null || enemies[i].HP <= 0) {
                    if (!IsPickable(enemies[i])) {
                        EventManager.TriggerEvent(EventNames.EnemyDied);
                    }
                    enemies.RemoveAt(i);
                    i--;
                }
            }

            attackCoroutine = null;
        }));
    }

    float GetDamage(Attackable enemy) {
        //TODO if enemy is boss and extraBossDamage==true -> extra damage
        return teamDamage;
    }

    bool IsPickable(Attackable enemy) {
        if (enemy != null) {
            if (enemy is Weapon weapon) {
                weapon.DisableBeenAttacked();
                return true;
            }
        }
        return false;
    }

    void HandleAttackVisualisation() {
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

    bool IsInAttackRange(Attackable enemy) =>
        enemy.transform.position.z - enemy.transform.lossyScale.z <= attackStartPosition;

    bool IsMainGuyReached(Attackable enemy) =>
        enemy.transform.position.z <= attackEndPosition;

    bool IsInAttackWidth(float position, float halfWidth) {
        int extraAttackZone = extraAttackWidth ? 1 : 0;
        return position >= MainGuy.Team.MostLeftMate.position.x - halfWidth - extraAttackZone
            && position <= MainGuy.Team.MostRightMate.position.x + halfWidth + extraAttackZone;
    }

    void AddEnemy(Attackable enemy) {
        if (enemies.Contains(enemy))
            return;
        enemies.Add(enemy);
    }

    void RemoveEnemy(Attackable enemy) {
        enemy.VisualiseTakeDamage(false);
        enemies.Remove(enemy);
    }

    void SubscriveEvents() =>
        EventManager.StartListening(EventNames.WeaponChanged, EventWeaponChanged);

    void UnsubscriveEvents() =>
        EventManager.StopListening(EventNames.WeaponChanged, EventWeaponChanged);
}
