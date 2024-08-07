using System.Collections.Generic;
using UnityEngine;

public class RoadObjectsGenerator : MonoBehaviour {
    public EnemySimple EnemySimplePrefab;
    public EnemyGiant EnemyGiantPrefab;
    public Cage CagePrefab;
    public Weapon BoxWithRocket;

    Dictionary<EnemyType, float> enemyWeights = new Dictionary<EnemyType, float>() {
        { EnemyType.Simple, 1 },
        { EnemyType.Giant, 5 },
    };
    
    public List<RoadObjectBase> GetObjects(
        int levelNumber,
        float roadLength,
        float roadWidth,
        List<float> roadTracksCoords,
        float budget
    ) {
        return DebugGenerateEnemies(roadTracksCoords);
    }

    List<RoadObjectBase> DebugObjectSetup(List<float> roadTracksCoords) {
        List<RoadObjectBase> objects = new List<RoadObjectBase>();
        float[] positions = new float[3] { 15, 35, 37 };
        foreach (float pos in positions) {
            Cage cage = Instantiate(CagePrefab);
            cage.RoadPosition = pos;
            cage.transform.Translate(
                new Vector3(
                    roadTracksCoords[Random.Range(0, roadTracksCoords.Count)],
                    cage.transform.localScale.y / 2,
                    0
                )
            );
            objects.Add(cage);
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
            nextPosition += Random.Range(1, 3);
            positions.Add(nextPosition);
        }

        foreach (float pos in positions) {
            var random = Random.Range(0, 7);
            RoadObjectBase nextPrefab = null;
            switch (random) {
                case 0: nextPrefab = EnemyGiantPrefab; break;
                case 1: nextPrefab = CagePrefab; break;
                case 2: nextPrefab = BoxWithRocket; break;
                default: nextPrefab = EnemySimplePrefab; break;
            }
            objects.Add(GetNewRoadItem(nextPrefab, pos, roadTracksCoords));
        }
        
        return objects;
    }
    RoadObjectBase GetNewRoadItem(RoadObjectBase prefab, float roadPosition, List<float> roadTracksCoords) {
        RoadObjectBase newItem = Instantiate(prefab);
        newItem.RoadPosition = roadPosition;
        newItem.transform.Translate(ProvideRandomPosition(roadTracksCoords, newItem));
        return newItem;
    }
    private Attackable ProvideWeaponBox(
        Weapon Prefab,
        float roadPosition,
        List<float> roadTracksCoords
    ) {
        Weapon weaponBox = Instantiate(Prefab);
        weaponBox.RoadPosition = roadPosition;
        weaponBox.transform.Translate(ProvideRandomPosition(roadTracksCoords, weaponBox));
        return weaponBox;
    }

    private Attackable ProvideEnemySimple(
        EnemySimple prefab,
        float roadPosition,
        List<float> roadTracksCoords
    ) {
        EnemySimple enemy = Instantiate(prefab);
        enemy.RoadPosition = roadPosition;
        enemy.transform.Translate(ProvideRandomPosition(roadTracksCoords, enemy));
        return enemy;
    }

    private Vector3 ProvideRandomPosition(
        List<float> roadTracksCoords,
        RoadObjectBase newObject
    ) {
        return new Vector3(
            roadTracksCoords[Random.Range(0, roadTracksCoords.Count)],
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