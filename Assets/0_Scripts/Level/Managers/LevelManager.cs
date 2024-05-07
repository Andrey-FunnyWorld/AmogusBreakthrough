using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Road Road;
    public RoadObjectsGenerator ObjectsGenerator;
    public MainGuy MainGuy;
    public LevelUIManager LevelUIManager;
    public AttackController AttackManager;
    public Cooldown EnemySpawnCooldown = new Cooldown(2); // calc spawn cooldown somehow

    void Start()
    {
        SubscriveEvents();
        Road.ZeroPointInWorld = MainGuy.transform.position.z;
        Road.AssignRoadObjects(ObjectsGenerator.GetObjects(0, Road.Length, Road.Width, 0));
    }

    void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space)) {
        //     Road.IsRunning = !Road.IsRunning;
        // }
        HandleSpawnEnemy();
    }

    void OnDestroy()
    {
        UnsubscriveEvents();
    }

    void LetsRoll()
    {
        MainGuy.StartMove();
        Road.IsRunning = true;
        LevelUIManager.LetsRoll();
        AttackManager.InitTeamDamage();
    }

    void StartMovement(object arg)
    {
        EventManager.StopListening(EventNames.StartMovement, StartMovement);
        LetsRoll();
    }

    void RoadFinished(object arg)
    {
        LevelUIManager.RoadFinished();
    }

    void EnemySpawned(object enemy)
    {
        if (enemy is RoadObjectBase)
        {
            Road.AddObject(enemy as RoadObjectBase);
        }
    }

    void SubscriveEvents()
    {
        EventManager.StartListening(EventNames.StartMovement, StartMovement);
        EventManager.StartListening(EventNames.RoadFinished, RoadFinished);
        EventManager.StartListening(EventNames.EnemySpawned, EnemySpawned);
    }

    void UnsubscriveEvents()
    {
        EventManager.StopListening(EventNames.StartMovement, StartMovement);
        EventManager.StopListening(EventNames.RoadFinished, RoadFinished);
        EventManager.StopListening(EventNames.EnemySpawned, EnemySpawned);
    }

    void HandleSpawnEnemy()
    {
        if (Road.IsRunning)
        {
            if (EnemySpawnCooldown.IsReady)
            {
                ObjectsGenerator.SpawnEnemySimple();
                EnemySpawnCooldown.Reset();
            }
        }
    }
}
