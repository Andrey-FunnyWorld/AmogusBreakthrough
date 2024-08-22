using System.Collections.Generic;
using UnityEngine;

public class AttackFXController : MonoBehaviour {

    public ParticleSystem RifleAttackFx;
    public ParticleSystem IonicAttackFx;
    public ParticleSystem BlasterAttackFx;

    private int maxEffectsCount;
    private bool processAttackVisualisation;
    private GameObject effectsSpawner;
    private ParticleSystem attackFx;
    private Transform leftEdge;
    private Transform rightEdge;
    private float startAttackPoint;
    private float endAttackPoint;
    private List<float> cooldowns;
    private Dictionary<WeaponType, GameObject> emitters = new Dictionary<WeaponType, GameObject>(3);

    private WeaponType currentWeaponType;
    private ImpactType currentWeaponImpactType;
    private List<Attackable> enemies;

    void Update() {
        if (processAttackVisualisation && attackFx != null) {
            VisualiseAttack();
        }
    }

    public void SetEnemies(List<Attackable> enemies) =>
        this.enemies = enemies;

    public bool IsPrepared(WeaponType weapon) =>
        processAttackVisualisation && currentWeaponType == weapon;
    void SetCurrentWeaponType(WeaponType weaponType) {
        currentWeaponType = weaponType;
    }
    public void PrepareAttackFXs(
        WeaponDefinition weapon,
        GameObject fxSpawner,
        Transform leftEdge,
        Transform rightEdge,
        float startAttackPoint,
        float endAttackPoint
    ) {
        SetCurrentWeaponType(weapon.Type);
        currentWeaponImpactType = weapon.ImpactType;
        this.leftEdge = leftEdge;
        this.rightEdge = rightEdge;
        this.startAttackPoint = startAttackPoint;
        this.endAttackPoint = endAttackPoint;

        StopPlayAnyFX();

        if (currentWeaponImpactType == ImpactType.Direct) {
            PlaySelectedDirectFX();
        } else {
            PrepareFXSpawnerObject(weapon.Type, fxSpawner);

            attackFx = effectsSpawner.GetComponent<ParticleSystem>();
            InitFXsCooldowns(weapon.FxEmissionFrequency);
        }
        
        processAttackVisualisation = true;
    }

    public void TeamSizeChanged(float width, float center) {
        var currentScale = RifleAttackFx.transform.localScale;
        var currentPosition = RifleAttackFx.transform.localPosition;
        RifleAttackFx.transform.localScale = new Vector3(width, currentScale.y, currentScale.z);
        RifleAttackFx.transform.localPosition = new Vector3(center, currentPosition.y, currentPosition.z);

        currentScale = IonicAttackFx.transform.localScale;
        currentPosition = IonicAttackFx.transform.localPosition;
        IonicAttackFx.transform.localScale = new Vector3(width, currentScale.y, currentScale.z);
        IonicAttackFx.transform.localPosition = new Vector3(center, currentPosition.y, currentPosition.z);

        currentScale = BlasterAttackFx.transform.localScale;
        currentPosition = BlasterAttackFx.transform.localPosition;
        BlasterAttackFx.transform.localScale = new Vector3(width, currentScale.y, currentScale.z);
        BlasterAttackFx.transform.localPosition = new Vector3(center, currentPosition.y, currentPosition.z);
    }

    public void ClearAttackFXs() {
        if (!processAttackVisualisation)
            return;

        StopPlayAnyFX();
        processAttackVisualisation = false;
        effectsSpawner = null;
        attackFx = null;
        leftEdge = null;
        rightEdge = null;
        SetCurrentWeaponType(WeaponType.Rifle);
    }

    private void PlaySelectedDirectFX() {
        StopPlayAnyFX();
        if (currentWeaponType == WeaponType.Rifle) {
            RifleAttackFx.Play();
        } else if (currentWeaponType == WeaponType.IonGun) {
            IonicAttackFx.Play();
        } else if (currentWeaponType == WeaponType.Blaster) {
            BlasterAttackFx.Play();
        }
    }

    private void StopPlayAnyFX() {
        RifleAttackFx.Stop();
        IonicAttackFx.Stop();
        BlasterAttackFx.Stop();
    }

    private void PrepareFXSpawnerObject(WeaponType weaponType, GameObject fxEmitter) {
        if (emitters.ContainsKey(weaponType)) {
            effectsSpawner = emitters[weaponType];
        } else {
            GameObject newEmitter = Instantiate(fxEmitter, transform);
            emitters[weaponType] = newEmitter;
            effectsSpawner = emitters[weaponType];
        }
    }

    private void VisualiseAttack() {
        if (enemies == null || enemies.Count == 0)
            return;
            
        for (int i = 0; i < maxEffectsCount; i++) {
            if (Time.time >= cooldowns[i]) {
                cooldowns[i] = GetNextEmitionTime();
                Vector3? fxSpawnPoint = GetFxSpawnPoint();
                if (fxSpawnPoint != null) {
                    effectsSpawner.transform.position = fxSpawnPoint.Value;
                    attackFx.Play();
                }
                
            }
        }
    }

    private void InitFXsCooldowns(int maxEffectsCount) {
        this.maxEffectsCount = maxEffectsCount;
        cooldowns = new List<float>(maxEffectsCount) { Time.time };
        for (int i = 1; i < maxEffectsCount; i++) {
            cooldowns.Add(GetNextEmitionTime());
        }
    }

    private float GetNextEmitionTime() =>
        Time.time + Random.Range(0.2f, 0.8f);

    private Vector3? GetFxSpawnPoint() {
        switch (currentWeaponImpactType) {
            case ImpactType.Direct:
                return GetFxSpawnPointDirectImpact();
            case ImpactType.Explosion:
                return GetPointNearRandomEnemy();
            default: return GetPointRandom();

        }
    }

    private Vector3? GetFxSpawnPointDirectImpact() {
        Vector3? pos = GetRandomEnemyPosition();
        if (pos != null)
            return new Vector3(pos.Value.x, pos.Value.y, pos.Value.z - 1.5f);
        else
            return null;
    }

    private Vector3 GetPointRandom() {
        return new Vector3(
            Random.Range(leftEdge.position.x, rightEdge.position.x),
            0,
            Random.Range(endAttackPoint + 2, startAttackPoint)
        );
    }

    private Vector3 GetPointNearRandomEnemy() {
        Vector3 pos = GetRandomEnemyPosition();
        return new Vector3(
            Random.Range(pos.x * 0.75f, pos.x * 1.25f),
            0,
            Random.Range(pos.z * 1.05f, pos.z * 0.95f)
        );
    }

    private Vector3 GetRandomEnemyPosition() {
        return enemies[Random.Range(0, enemies.Count)]?.transform.position ?? GetPointRandom();
    }
}
