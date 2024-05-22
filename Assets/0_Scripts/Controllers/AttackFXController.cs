using System.Collections.Generic;
using UnityEngine;

public class AttackFXController : MonoBehaviour {

    public int MaxEffectsCount = 3;

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
    private List<EnemyBase> enemies;

    void Update() {
        if (processAttackVisualisation && attackFx != null) {
            VisualiseAttack();
        }
    }

    public void SetEnemies(List<EnemyBase> enemies) {
        this.enemies = enemies;
    }

    public bool IsPrepared(WeaponType weapon) {
        return processAttackVisualisation && currentWeaponType == weapon;
    }

    public void PrepareAttackFXs(
        WeaponType weaponType,
        GameObject fxSpawner,
        Transform leftEdge,
        Transform rightEdge,
        float startAttackPoint,
        float endAttackPoint
    ) {
        currentWeaponType = weaponType;
        this.leftEdge = leftEdge;
        this.rightEdge = rightEdge;
        this.startAttackPoint = startAttackPoint;
        this.endAttackPoint = endAttackPoint;
        PrepareFXSpawnerObject(weaponType, fxSpawner);

        attackFx = effectsSpawner.GetComponent<ParticleSystem>();
        InitFXsCooldowns();
        processAttackVisualisation = true;
    }

    public void ClearAttackFXs() {
        if (!processAttackVisualisation)
            return;
            
        processAttackVisualisation = false;
        effectsSpawner = null;
        attackFx = null;
        leftEdge = null;
        rightEdge = null;
        currentWeaponType = WeaponType.NoWeapon;
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
        for (int i = 0; i < MaxEffectsCount; i++) {
            if (Time.time >= cooldowns[i]) {
                cooldowns[i] = GetNextEmitionTime();
                Vector3 fxSpawnPoint = GetFxSpawnPoint();
                effectsSpawner.transform.position = fxSpawnPoint;
                attackFx.Play();
            }
        }
    }

    private void InitFXsCooldowns() {
        cooldowns = new List<float>(MaxEffectsCount) { Time.time };
        for (int i = 1; i < MaxEffectsCount; i++) {
            cooldowns.Add(GetNextEmitionTime());
        }
    }

    private float GetNextEmitionTime() {
        return Time.time + Random.Range(0.2f, 0.8f);
    }

    private Vector3 GetFxSpawnPoint() {
        return enemies.Count > 0
            ? GetPointNearRandomEnemy()
            : GetPointRandom();
    }

    private Vector3 GetPointRandom() {
        return new Vector3(
            Random.Range(leftEdge.position.x, rightEdge.position.x),
            0,
            Random.Range(endAttackPoint + 2, startAttackPoint)
        );
    }

    private Vector3 GetPointNearRandomEnemy() {
        Vector3 pos = enemies[Random.Range(0, enemies.Count)].transform.position;
        return new Vector3(
            Random.Range(pos.x * 0.75f, pos.x * 1.25f),
            0,
            Random.Range(pos.z * 1.05f, pos.z * 0.95f)
        );
    }
}
