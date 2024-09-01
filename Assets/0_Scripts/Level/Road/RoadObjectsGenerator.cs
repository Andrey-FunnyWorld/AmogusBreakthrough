using System;
using System.Collections.Generic;
using UnityEngine;

public class RoadObjectsGenerator : MonoBehaviour {
    public EnemySimple EnemySimplePrefab;
    public EnemyGiant EnemyGiantPrefab;
    public Cage CagePrefab;
    public Weapon WeaponBoxPrefab;
    public RoadObstacle ObstaclePrefab;
    public Transform enemiesContainer;

    public GameObject IonGunPrefab;
    public GameObject BlasterPrefab;
    public GameObject RifflePrefab;
    public GameObject BazookaPrefab;
    
    public List<RoadObjectBase> GetObjects(
        RoadDataViewModel vm,
        float roadLength,
        float roadWidth,
        List<float> roadTracksCoords
    ) {
        return GenerateRealObjects(vm, roadTracksCoords);
        //return GenerateObstacles(roadTracksCoords);
        //return DebugGenerateEnemies(roadTracksCoords);
    }
    
    List<RoadObjectBase> GenerateRealObjects(RoadDataViewModel vm, List<float> roadTracksCoords) {
        List<RoadObjectBase> objects = new List<RoadObjectBase>();
        foreach (RoadObjectViewModel ovm in vm.Objects) {
            objects.Add(CreateObject(ovm, GetXOnRoad(ovm.TrackNo, roadTracksCoords)));
        }
        return objects;
    }
    RoadObjectBase CreateObject(RoadObjectViewModel ovm, float x) {
        RoadObjectBase newItem = Instantiate(GetPrefabByObjectType(ovm.RoadObjectType));
        AdjustNewItem(newItem);
        newItem.RoadPosition = ovm.Position;
        newItem.transform.Translate(new Vector3(x, 0, 0));
        newItem.transform.parent = enemiesContainer;
        return newItem;
    }
    void AdjustNewItem(RoadObjectBase newItem) {
        if (newItem is Weapon weapon) {
            weapon.TurnOffDieFx();
            weapon.WeaponType = GetRandomWeapon();
            GenerateWeaponForBox(weapon.weaponMarker.transform, weapon.WeaponType);
        }
    }
    float GetXOnRoad(int trackNo, List<float> roadTracksCoords) {
        int index = trackNo == -1 ? UnityEngine.Random.Range(0, roadTracksCoords.Count) : trackNo;
        return roadTracksCoords[index];
    }
    RoadObjectBase GetPrefabByObjectType(RoadObjectType objectType) {
        switch (objectType) {
            case RoadObjectType.EnemySimple: return EnemySimplePrefab;
            case RoadObjectType.EnemyGiant: return EnemyGiantPrefab;
            case RoadObjectType.Cage: return CagePrefab;
            case RoadObjectType.ObstacleSaw: return ObstaclePrefab;
            case RoadObjectType.WeaponBox: return WeaponBoxPrefab;
        }
        return null;
    }
    #region DEBUG GENERATION
    List<RoadObjectBase> DebugObjectSetup(List<float> roadTracksCoords) {
        List<RoadObjectBase> objects = new List<RoadObjectBase>();
        float[] positions = new float[3] { 15, 35, 37 };
        foreach (float pos in positions) {
            Cage cage = Instantiate(CagePrefab);
            cage.RoadPosition = pos;
            cage.transform.Translate(
                new Vector3(
                    roadTracksCoords[UnityEngine.Random.Range(0, roadTracksCoords.Count)],
                    cage.transform.localScale.y / 2,
                    0
                )
            );
            objects.Add(cage);
        }
        return objects;
    }
    List<RoadObjectBase> GenerateObstacles(List<float> roadTracksCoords) {
        List<RoadObjectBase> objects = new List<RoadObjectBase>(10);
        for (int i = 0; i < 10; i++) {
            RoadObjectBase roadObject = GetNewRoadItem(ObstaclePrefab, 10 + i * 5, roadTracksCoords);
            roadObject.transform.position = new Vector3(roadTracksCoords[9 - i], 0, 0);
            objects.Add(roadObject);
        }
        return objects;
    }
    List<RoadObjectBase> DebugGenerateEnemies(List<float> roadTracksCoords) {
        //int enemiesCount = Random.Range(20, 50);
        int enemiesCount = 30;
        List<RoadObjectBase> objects = new List<RoadObjectBase>(enemiesCount);
        float nextPosition = 15;
        List<float> positions = new List<float>(enemiesCount) { nextPosition };
        
        for (int i = 1; i < enemiesCount; i++) {
            nextPosition += UnityEngine.Random.Range(1, 3);
            positions.Add(nextPosition);
        }

        foreach (float pos in positions) {
            var random = UnityEngine.Random.Range(0, 7);
            RoadObjectBase nextPrefab = null;
            switch (random) {
                case 0: nextPrefab = EnemyGiantPrefab; break;
                case 1: nextPrefab = CagePrefab; break;
                case 2: nextPrefab = WeaponBoxPrefab; break;
                default: nextPrefab = EnemySimplePrefab; break;
            }
            objects.Add(GetNewRoadItem(nextPrefab, pos, roadTracksCoords));
        }
        
        return objects;
    }
    #endregion
    RoadObjectBase GetNewRoadItem(RoadObjectBase prefab, float roadPosition, List<float> roadTracksCoords) {
        RoadObjectBase newItem = Instantiate(prefab);
        newItem.RoadPosition = roadPosition;
        newItem.transform.Translate(ProvideRandomPosition(roadTracksCoords, newItem));
        if (newItem is Weapon weapon) {
            weapon.TurnOffDieFx();
            weapon.WeaponType = GetRandomWeapon();
            GenerateWeaponForBox(weapon.weaponMarker.transform, weapon.WeaponType);
        }
        newItem.transform.parent = enemiesContainer;
        return newItem;
    }
    WeaponType GetRandomWeapon() {
        var values = Enum.GetValues(typeof(WeaponType));
        return (WeaponType)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }
    void GenerateWeaponForBox(Transform transform, WeaponType weapon) {
        GameObject newInstance = null;
        if (weapon == WeaponType.Rifle) {
            newInstance = Instantiate(RifflePrefab, transform);
        } else if (weapon == WeaponType.Blaster) {
            newInstance = Instantiate(BlasterPrefab, transform);
        } else if (weapon == WeaponType.Bazooka) {
            newInstance = Instantiate(BazookaPrefab, transform);
        } else if (weapon == WeaponType.IonGun) {
            newInstance = Instantiate(IonGunPrefab, transform);
        }
        transform.gameObject.SetActive(false);
    }
    // private Attackable ProvideWeaponBox(
    //     Weapon Prefab,
    //     float roadPosition,
    //     List<float> roadTracksCoords
    // ) {
    //     Weapon weaponBox = Instantiate(Prefab);
    //     weaponBox.RoadPosition = roadPosition;
    //     weaponBox.transform.Translate(ProvideRandomPosition(roadTracksCoords, weaponBox));
    //     return weaponBox;
    // }

    // private Attackable ProvideEnemySimple(
    //     EnemySimple prefab,
    //     float roadPosition,
    //     List<float> roadTracksCoords
    // ) {
    //     EnemySimple enemy = Instantiate(prefab);
    //     enemy.RoadPosition = roadPosition;
    //     enemy.transform.Translate(ProvideRandomPosition(roadTracksCoords, enemy));
    //     return enemy;
    // }

    private Vector3 ProvideRandomPosition(
        List<float> roadTracksCoords,
        RoadObjectBase newObject
    ) {
        return new Vector3(
            roadTracksCoords[UnityEngine.Random.Range(0, roadTracksCoords.Count)],
            //enemy.transform.localScale.y / 2,
            0,
            0
        );
    }

}

public enum EnemyType {
    Simple, Giant
}
public enum BonusType {
    Cage, WeaponBlaster
}

public class ObjectPosition {
    public EnemyBase Enemy;
    public float RoadPosition;
}