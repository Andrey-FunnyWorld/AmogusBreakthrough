using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.Events;

public class DesintegratorScene : MonoBehaviour {
    public MenuCameraController CameraController;
    public CinemachineBrain CameraBrain;
    public EnemySimple EnemySimplePrefab;
    public EnemyGiant EnemyGiantPrefab;
    public int SimpleEnemies = 5;
    public int SimpleGiants = 2;
    public Bounds EnemyBounds;
    public float StartDelay = 1;
    public float EndDelay = 2.5f;
    public Desintegrator Desintegrator;
    public Transform WavePosition;
    float originalCameraDuratio = 0;
    List<EnemyBase> enemies = new List<EnemyBase>();
    public void Show(UnityAction afterSceneAction) {
        GenerateEnemies();
        originalCameraDuratio = CameraBrain.m_DefaultBlend.m_Time;
        CameraBrain.m_DefaultBlend.m_Time = 0;
        CameraController.SwitchToCamera((int)CameraType.DesintegratorScene);
        StartCoroutine(Utils.ChainActions(new List<ChainedAction>() {
            new ChainedAction() { DeltaTime = StartDelay, Callback = () => { 
                Desintegrator.Wave(WavePosition.position);
            }},
            new ChainedAction() { DeltaTime = 0.15f, Callback = () => {
                foreach (EnemyBase enemy in enemies) {
                    enemy.HP = 0;
                }
            }},
            new ChainedAction() { DeltaTime = EndDelay, Callback = () => {
                CameraController.SwitchToCamera((int)CameraType.Desintegrator);
            }},
            new ChainedAction() { DeltaTime = originalCameraDuratio, Callback = () => {
                CameraBrain.m_DefaultBlend.m_Time = originalCameraDuratio;
                afterSceneAction.Invoke();     
            }}
        }));
    }
    void GenerateEnemies() {
        for (int i = 0; i < SimpleEnemies; i++) {
            EnemyBase enemy = Instantiate(EnemySimplePrefab, transform);
            enemy.transform.position = GetRandomPosition();
            enemies.Add(enemy);
        }
        for (int i = 0; i < SimpleGiants; i++) {
            EnemyBase enemy = Instantiate(EnemyGiantPrefab, transform);
            enemy.transform.position = GetRandomPosition();
            enemies.Add(enemy);
        }
    }
    Vector3 GetRandomPosition() {
        float x = Random.Range(EnemyBounds.min.x, EnemyBounds.max.x);
        float z = Random.Range(EnemyBounds.min.y, EnemyBounds.max.y);
        return new Vector3(x, 0, z) + transform.position;
    }
}
