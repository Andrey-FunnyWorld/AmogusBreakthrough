using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class RoadDataGenerator : MonoBehaviour {
    public RoadDataStorage RoadDataStorage;
    public float MinRoadLength = 70;
    public float MaxRoadLength = 200;
    public float StartRoadPosition = 5;
    public float EndRoadPositionIndent = 10;
    public int MinGroupCount = 7;
    public int MaxGroupCount = 20;
    public float MinGroupIndent = 5;
    public float MinBudget = 25;
    public float BudgetStep = 15;
    public float RoadLengthStep = 15;
    public float MinGroupBudget = 2;
    const int MIN_OBSTACLE_TRACK_INDENT = 2; // a group of 10 amoguses can bypass
    const int TRACKS = 10;
    const int TEAM_START_COUNT = 2;
    const int MAX_TEAM_COUNT = 10;
    Dictionary<Difficulty, float> difficultFactor = new Dictionary<Difficulty, float>() {
        { Difficulty.Noob, 0.8f },
        { Difficulty.Pro, 1f },
        { Difficulty.Hacker, 0.3f },
    };
    Dictionary<RoadObjectType, float> enemyWeights = new Dictionary<RoadObjectType, float>() {
        { RoadObjectType.EnemySimple, 1 },
        { RoadObjectType.EnemyGiant, 5 }
    };
    public RoadDataViewModel GetLevelViewModel(int levelNo, Difficulty difficulty) {
        RoadDataViewModel vm = RoadDataStorage.Levels.FirstOrDefault(l => l.LevelNo == levelNo);
        vm = vm == null ? GenerateLevelData(levelNo, difficulty) : vm;
        System.Array.Sort(vm.Objects, (RoadObjectViewModel a, RoadObjectViewModel b) => { return a.Position.CompareTo(b.Position); });
        return vm;
    }
    RoadDataViewModel GenerateLevelData(int levelNo, Difficulty difficulty) {
        RoadDataViewModel vm = new RoadDataViewModel() { LevelNo = levelNo };
        vm.Length = GetRoadLength(levelNo);
        vm.Objects = GetRoadObjects(levelNo, difficulty, vm.Length).ToArray();
        return vm;
    }
    List<RoadObjectViewModel> GetRoadObjects(int levelNo, Difficulty difficulty, float length) {
        float budget = GetBudget(levelNo, difficulty);
        return SpendBudget(budget, length);
    }
    float GetBudget(int levelNo, Difficulty difficulty) {
        return (MinBudget + (levelNo / 4) * BudgetStep) * difficultFactor[difficulty];
    }
    float GetRoadLength(int levelNo) {
        return Mathf.Min(MaxRoadLength, MinRoadLength + (levelNo / 2) * RoadLengthStep);
    }
    List<RoadObjectViewModel> SpendBudget(float budget, float length) {
        List<RoadObjectViewModel> objects = new List<RoadObjectViewModel>();
        int groupCount = UnityEngine.Random.Range(MinGroupCount, (int)Mathf.Min(MaxGroupCount, Mathf.Floor(budget / MinGroupBudget)));
        List<float> groupPositions = GetGroupPositions(groupCount, length);
        List<float> groupBudgets = GetGroupBudgets(groupCount, budget);
        List<GroupRect> groupRects = new List<GroupRect>(groupCount);
        for (int i = 0; i < groupCount; i++) {
            GroupLayout layout = GetGroupLayout();
            GroupRect rect = GetGroupRect(layout, groupPositions[i]);
            groupRects.Add(rect);
            objects.AddRange(GetGroup(groupBudgets[i], layout, rect));
        }
        List<RoadObjectViewModel> bonuses = GenerateBonuses(groupRects);
        objects.AddRange(bonuses);
        objects.AddRange(GenerateObstacles(groupRects, bonuses));
        return objects;
    }
    List<float> GetGroupPositions(int groupCount, float length) {
        float avgDistanceBetweenGroups = (length - StartRoadPosition - EndRoadPositionIndent) / (groupCount - 1);
        float maxGroupIndent = avgDistanceBetweenGroups * 2 - MinGroupIndent;
        float groupIndentDelta = (maxGroupIndent - MinGroupIndent) / (groupCount - 1);
        List<float> groupPositions = new List<float>(groupCount) { StartRoadPosition };
        float currentDelta = maxGroupIndent;
        for (int i = 1; i < groupCount; i++) {
            groupPositions.Add(groupPositions[i - 1] + currentDelta);
            currentDelta -= groupIndentDelta;
        }
        return groupPositions;
    }
    List<float> GetGroupBudgets(int groupCount, float budget) {
        float avgGroupBudget = budget / groupCount;
        List<float> groupBudgets = new List<float>(groupCount);
        for (int i = 0; i < groupCount; i++) {
            groupBudgets.Add(avgGroupBudget);
        }
        return groupBudgets;
    }
    List<RoadObjectViewModel> GetGroup(float budget, GroupLayout layout, GroupRect rect) {
        List<RoadObjectViewModel> objects = new List<RoadObjectViewModel>();
        if (layout == GroupLayout.Quad) {
            float budgetNorm = Mathf.Ceil(budget);
            while (budgetNorm > 0) {
                objects.Add(GetNextEnemy(budgetNorm, rect));
                budgetNorm -= enemyWeights[objects.Last().RoadObjectType];
            }
        }
        return objects;
    }
    GroupRect GetGroupRect(GroupLayout layout, float position) {
        GroupRect rect = null;
        if (layout == GroupLayout.Quad) {
            float height = Random.Range(0, MinGroupIndent);
            int width = Random.Range(1, 11);
            int startTrackNo = Random.Range(0, TRACKS - width);
            bool isLeft = Random.Range(0, 2) == 0;
            rect = new GroupRect() {
                MinPosition = position - height / 2,
                MaxPosition = position + height / 2,
                MinTrackNo = isLeft ? startTrackNo : TRACKS - startTrackNo - width,
                MaxTrackNo = isLeft ? startTrackNo + width : TRACKS - startTrackNo
            };
        }
        return rect;
    }
    GroupLayout GetGroupLayout() {
        return GroupLayout.Quad;
        GroupLayout[] layouts = System.Enum.GetValues(typeof(GroupLayout)).Cast<GroupLayout>().ToArray();
        return layouts[UnityEngine.Random.Range(0, layouts.Length)];
    }
    RoadObjectViewModel GetNextEnemy(float budgetLeft, GroupRect rect) {
        List<RoadObjectType> enemyTypes = new List<RoadObjectType>();
        foreach (KeyValuePair<RoadObjectType, float> pair in enemyWeights) {
            if (pair.Value <= budgetLeft) enemyTypes.Add(pair.Key);
        }
        RoadObjectType objectType = enemyTypes[Random.Range(0, enemyTypes.Count)];
        float position = Random.Range(rect.MinPosition, rect.MaxPosition);
        int trackNo = Random.Range(rect.MinTrackNo, rect.MaxTrackNo);
        return new RoadObjectViewModel() {
            RoadObjectType = objectType,
            Position = position,
            TrackNo = trackNo
        };
    }
    List<RoadObjectViewModel> GenerateObstacles(List<GroupRect> groupRects, List<RoadObjectViewModel> bonuses) {
        List<RoadObjectViewModel> objects = new List<RoadObjectViewModel>();
        const float trackPartByTeamMember = (TRACKS - MIN_OBSTACLE_TRACK_INDENT) / MAX_TEAM_COUNT;
        for (int i = 0; i < groupRects.Count - 1; i++) {
            float position = Random.Range(groupRects[i].MaxPosition, groupRects[i + 1].MinPosition);
            int cagesPassed = bonuses.Count(b => b.RoadObjectType == RoadObjectType.Cage && b.Position < position);
            int potentialTeamCount = TEAM_START_COUNT + cagesPassed;
            int maxIndent = MIN_OBSTACLE_TRACK_INDENT + (int)Mathf.Floor((MAX_TEAM_COUNT - potentialTeamCount) * trackPartByTeamMember);
            int trackNo = Random.Range(0, maxIndent);
            bool isLeft = Random.Range(0, 2) == 0;
            trackNo = isLeft ? maxIndent : TRACKS - maxIndent;
            RoadObjectViewModel ovm = new RoadObjectViewModel() {
                RoadObjectType = RoadObjectType.ObstacleSaw,
                TrackNo = trackNo,
                Position = position
            };
            objects.Add(ovm);
        }
        return objects;
    }
    List<RoadObjectViewModel> GenerateBonuses(List<GroupRect> groupRects) {
        List<RoadObjectViewModel> objects = new List<RoadObjectViewModel>();
        for (int i = 0; i < groupRects.Count - 1; i++) {
            RoadObjectType bonusType = Random.Range(0, 2) == 0 ? RoadObjectType.Cage : RoadObjectType.WeaponBox;
            RoadObjectViewModel ovm = new RoadObjectViewModel() {
                RoadObjectType = bonusType,
                TrackNo = Random.Range(0, TRACKS)
            };
            bool isInside = Random.Range(0, 2) == 0;
            ovm.Position = isInside ? groupRects[i].GetRandomPosition()
                : Random.Range(groupRects[i].MaxPosition, groupRects[i + 1].MinPosition);
            objects.Add(ovm);
        }
        return objects;
    }
}
[System.Serializable]
public class RoadObjectViewModel {
    [Min(5)]
    public float Position;
    public RoadObjectType RoadObjectType;
    [Range(-1, 9)]
    public int TrackNo = -1;
}
[System.Serializable]
public class RoadDataViewModel {
    public RoadObjectViewModel[] Objects;
    public float Length;
    public int LevelNo;
    public override string ToString() {
        return string.Format("RoadDataViewModel: Length={0}; LevelNo:{1}; Objects={2};", Length, LevelNo, Objects.Length);
    }
}

public enum RoadObjectType {
    EnemySimple, EnemyGiant, ObstacleSaw, Cage, WeaponBox
}
public enum GroupLayout {
    Quad, Line, ZigZag
}
public class GroupRect {
    public int MinTrackNo, MaxTrackNo;
    public float MinPosition, MaxPosition;
    public float GetRandomPosition() {
        return Random.Range(MinPosition, MaxPosition);
    }
}