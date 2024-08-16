using System.Collections.Generic;
using UnityEngine;

public class AttackController : MonoBehaviour {
    public Road Road;
    public MainGuy MainGuy;
    public WeaponsList weaponsStaticData;
    public AttackFXController fxController;
    public GameObject AttackZonePlane;
    public Transform AttackZoneParent;
    public ShotSoundManager ShotSoundManager;
    public float OnePunchZone = 20f;

    public float bossExtraMin = 1.1f;
    public float bossExtraMax = 1.4f;

    List<Attackable> enemies = new List<Attackable>(20);
    Dictionary<WeaponType, WeaponDefinition> weapons;
    Coroutine attackCoroutine = null;
    WeaponDefinition currentWeapon;

    float attackStartPosition;
    float attackEndPosition;
    float teamDamage = 0.5f;
    float attackOnePunchStartPosition;

    bool extraBossDamage;
    bool extraAttackWidth;

    void Start() {
        SubscriveEvents();
        fxController.SetEnemies(enemies);
    }

    void Update() {
        if (Road.MovementStarted && enemies.Count > 0) {
            AttackEnemies();
            ShotSoundManager.Play();
        } else if (!Road.MovementStarted) {
            ShotSoundManager.Stop();
            enemies.Clear();
            fxController.ClearAttackFXs();
        } else {
            ShotSoundManager.Stop();
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
        InitAttackOnePunchRange();
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
        if (perk == PerkType.BossDamage) {
            extraBossDamage = true;
        } else if (perk == PerkType.AttackZoneVisibility) {
            CalcAndActivateAttackZoneVisibility();
        } else if (perk == PerkType.ExtraAttackWidth) {
            extraAttackWidth = true;
            CalcAttackZone();
        }
    }

    void CalcAttackZone() {
        Vector3 teamSize = MainGuy.Team.GetTeamSize(extraAttackWidth);
        Vector3 zoneParentPrevLocal = AttackZoneParent.localPosition;
        Vector3 teamPosition = MainGuy.Team.GetTeamPosition();
        Vector3 worldToLocal = MainGuy.transform.InverseTransformPoint(teamPosition);

        AttackZoneParent.localPosition = new Vector3(worldToLocal.x, zoneParentPrevLocal.y, zoneParentPrevLocal.z);
        AttackZonePlane.transform.localPosition = new Vector3(0, 0, teamSize.z / 2);
        AttackZonePlane.transform.localScale = new Vector3(teamSize.x, 0.01f, teamSize.z);

        fxController.TeamSizeChanged(teamSize.x, worldToLocal.x);
    }

    void CalcAndActivateAttackZoneVisibility() {
        CalcAttackZone();
        AttackZonePlane.transform.parent.gameObject.SetActive(true);
    }

    private void HandleMatesChanged(object arg) {
        CalcAttackZone();
        MainGuy.Team.SwitchWeapon(currentWeapon.Type);
    }

    public void HandleOnePunchAbility(List<RoadObjectBase> objects) {
        foreach (RoadObjectBase obj in objects) {
            if (obj is EnemyBase enemy) {
                if (IsInOnePunchRange(enemy))
                    enemy.HP = 0;
            }
        }
    }

    bool IsInOnePunchRange(EnemyBase enemy) {
        return enemy.transform.position.z - enemy.transform.lossyScale.z <= attackOnePunchStartPosition
            && enemy.transform.position.z > attackEndPosition;
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

    void InitAttackOnePunchRange() {
        attackOnePunchStartPosition = MainGuy.transform.position.z + OnePunchZone + 1;
    }

    void PrepareWeapons() {
        weapons = new Dictionary<WeaponType, WeaponDefinition>(weaponsStaticData.Items.Length);
        foreach (WeaponDefinition weapon in weaponsStaticData.Items)
            weapons.Add(weapon.Type, weapon);
        currentWeapon = weapons[MainGuy.CurrentWeaponType];
    }

    void HandleWeaponChanged(WeaponType weaponType) {
        currentWeapon = weapons[weaponType];
        InitTeamDamage();
        MainGuy.SwitchWeapon(weaponType);
        ShotSoundManager.SetWeapon(weaponType);
    }

    void InitTeamDamage() =>
        teamDamage = currentWeapon.Damage * MainGuy.DamageMultiplier
            + MainGuy.Team.MatesCount * currentWeapon.Damage;

    void AttackEnemies() {
        if (attackCoroutine != null) return;

        attackCoroutine = StartCoroutine(Utils.WaitAndDo(currentWeapon.AttackCooldown, () => {
            for (int i = 0; i < enemies.Count; i++) {

                if (enemies[i] == null) {
                    enemies.RemoveAt(i);
                    i--;
                    continue;
                }

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
        if (extraBossDamage && enemy.GetComponent<EnemyGiant>() != null) {
            return teamDamage * UnityEngine.Random.Range(bossExtraMin, bossExtraMax);
        }
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

    void SubscriveEvents() {
        EventManager.StartListening(EventNames.WeaponChanged, EventWeaponChanged);
        EventManager.StartListening(EventNames.MatesChanged, HandleMatesChanged);
    }

    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.WeaponChanged, EventWeaponChanged);
        EventManager.StopListening(EventNames.MatesChanged, HandleMatesChanged);
    }
}
