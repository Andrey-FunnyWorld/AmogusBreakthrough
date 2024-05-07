using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadObjectsGenerator : MonoBehaviour
{
    public EnemySimple EnemySimplePrefab;
    public EnemyGiant EnemyGiantPrefab;
    public Cage CagePrefab;
    public GameObject EnemiesContainer;

    Dictionary<EnemyType, float> enemyWeights = new Dictionary<EnemyType, float>() {
        { EnemyType.Simple, 1 },
        { EnemyType.Giant, 5 },
    };

    public List<RoadObjectBase> GetObjects(int levelNumber, float roadLength, float roadWidth, float budget)
    {
        return DebugObjectSetup(roadWidth);
    }

    List<RoadObjectBase> DebugObjectSetup(float width)
    {
        List<RoadObjectBase> objects = new List<RoadObjectBase>();
        float[] positions = new float[3] { 10, 20, 30 };
        foreach (float pos in positions)
        {
            Cage cage = Instantiate(CagePrefab);
            cage.RoadPosition = pos;
            cage.transform.Translate(new Vector3(Random.Range(-width / 2, width / 2), 0, 0));
            objects.Add(cage);
        }
        return objects;
    }

    public void SpawnEnemySimple()
    {
        EnemySimple enemy = Instantiate(EnemySimplePrefab);
        enemy.transform.Translate(new Vector3(Random.Range(-5, 5), 1, Random.Range(30, 50)));
        enemy.transform.parent = EnemiesContainer.transform;
        EventManager.TriggerEvent(EventNames.EnemySpawned, enemy);
    }

}

public enum EnemyType
{
    Simple, Giant
}
public enum BonusType
{
    Cage, WeaponBlaster
}

public class ObjectPosition
{
    public EnemyBase Enemy;
    public float RoadPosition;
}