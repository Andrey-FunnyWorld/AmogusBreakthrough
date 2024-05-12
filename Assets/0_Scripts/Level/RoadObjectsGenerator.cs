using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadObjectsGenerator : MonoBehaviour {
    public EnemySimple EnemySimplePrefab;
    public EnemyGiant EnemyGiantPrefab;
    public Cage CagePrefab;

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
        // return DebugObjectSetup(roadTracksCoords);
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
        List<RoadObjectBase> objects = new List<RoadObjectBase>();
        float nextPosition = 15;
        int enemies = Random.Range(20, 50);
        List<float> positions = new List<float> { nextPosition };
        
        for (int i = 0; i < enemies; i++) {
            nextPosition += Random.Range(1, 3);
            positions.Add(nextPosition);
        }

        foreach (float pos in positions) {
            EnemySimple enemy = Instantiate(EnemySimplePrefab);
            enemy.RoadPosition = pos;
            enemy.transform.Translate(
                new Vector3(
                    roadTracksCoords[Random.Range(0, roadTracksCoords.Count)],
                    enemy.transform.localScale.y / 2,
                    0
                )
            );
            objects.Add(enemy);
        }
        return objects;
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