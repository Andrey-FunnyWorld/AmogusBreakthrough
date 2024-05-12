using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class Team : MonoBehaviour {
    public MainGuy MainGuy;
    public Amogus AmogusPrefab;
    [HideInInspector]
    public int Count;
    public int StartupCount = 3;
    public Material SourceMaterial;
    List<Color> colors;
    List<Amogus> Mates;
    Dictionary<Color, Material> Materials = new Dictionary<Color, Material>();
    MyRange teamRange = new MyRange();
    
    public MyRange TeamRange { get { return teamRange; } }

    public float AttackRange = 10f;
    [NonSerialized] public Transform MostLeftMate;
    [NonSerialized] public Transform MostRightMate;

    const float X_OFFSET = 0.6f;
    const float Z_OFFSET = 0.5f;
    const float MIN_RANGE = 0.4f;
    const int MAX_CAPACITY = 10;

    void Start() {
        SubscriveEvents();
    }
    void OnDestroy() {
        UnsubscriveEvents();
    }

    public void CreateTeam() {
        Mates = new List<Amogus>(StartupCount);
        colors = new List<Color>(StartupCount);
        for (int i = 0; i < StartupCount; i++) {
            CreateMate();
        }
        CalcTeamRange();
    }
    public void AddNewMate() {
        CreateMate();
        CalcTeamRange();
    }
    void CreateMate() {
        Amogus newMate = Instantiate(AmogusPrefab, transform);
        Vector3 offset = new Vector3(
            X_OFFSET * (Mates.Count / 2 + 1) * (Mates.Count % 2 == 0 ? -1 : 1),
            0,
            Z_OFFSET * ((Mates.Count / 2) % 2 == 0 ? -1 : 1)
        );
        newMate.PositionOffset = offset;
        newMate.transform.Translate(offset + new Vector3(MainGuy.transform.position.x, 0, 0));
        colors.Add(GetNextColor());
        newMate.SetColor(GetMaterial(colors.Last()));
        Mates.Add(newMate);

        if (MostLeftMate == null || MostLeftMate.position.x > offset.x) {
            MostLeftMate = newMate.transform;
        }
        if (MostRightMate == null || MostRightMate.position.x < offset.x) {
            MostRightMate = newMate.transform;
        }
    }
    Material GetMaterial(Color color) {
        if (!Materials.ContainsKey(color)) {
            Material newMaterial = new Material(SourceMaterial);
            newMaterial.SetColor("_Color", color);
            Materials.Add(color, newMaterial);
        }
        return Materials[color];
    }
    void CalcTeamRange() {
        teamRange.Start = Mathf.Min(-MIN_RANGE, -X_OFFSET * ((Mates.Count + 1) / 2));
        teamRange.End = Mathf.Max(MIN_RANGE, X_OFFSET * (Mates.Count / 2));
        // Debug.Log($"Team range changed: start:{teamRange.Start}, end:{teamRange.End}");

        // teamBounds = CalcCombinedBounds(gameObject);
        // Debug.Log($"combined bounds center: {teamBounds.center}, size:{teamBounds.size}");
    }
    public void ApplyMovement(Vector3 newPosition) {
        foreach (Amogus amogus in Mates) {
            amogus.ApplyMovement(newPosition);
        }
    }
    Color GetNextColor() {
        List<Color> availableColors = TeamColors.Except(colors).ToList();
        return availableColors[UnityEngine.Random.Range(0, availableColors.Count)];
    }
    static Color[] TeamColors = new Color[10] {
        Color.red, Color.blue, Color.green, Color.magenta, Color.yellow, Color.grey,
        new Color(1, 0.49f, 0.19f), new Color(0.54f, 0.17f, 0.88f), new Color(0.57f, 0.44f, 0.86f), new Color(0.5f, 0.5f,0)
    };
    
    #region Events
    void SubscriveEvents() {
        EventManager.StartListening(EventNames.CageDestroyed, CageDestroyed);
    }
    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.CageDestroyed, CageDestroyed);
    }
    void CageDestroyed(object arg) {
        if (Count == MAX_CAPACITY) Debug.LogError("Cage Destroyed: TEAM ALREADY FULL");
        else {
            Cage cage = (Cage)arg;
            AddNewMate();
        }
    }
    #endregion

    void OnDrawGizmos() {
        if (MostLeftMate != null) {
            Gizmos.color = new Color(1, 1, 0, 0.25F);
            Gizmos.DrawCube(GetTeamPosition(), GetTeamSize());
        }
    }
    private Vector3 GetTeamPosition() {
        return new Vector3(
            (MostLeftMate.position.x + MostRightMate.position.x) / 2,
            2,
            transform.position.z + AttackRange / 2 + 1
        );
    }
    private Vector3 GetTeamSize() {
        return new Vector3(Mathf.Abs(teamRange.Start) + Mathf.Abs(teamRange.End), 4, AttackRange);
    }
}

public class MyRange {
    public float Start;
    public float End;
    public override string ToString() {
        return string.Format("Start: {0} | End: {1}", Start, End);
    }
}