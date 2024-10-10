using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoadDataGenerator : MonoBehaviour {
    public RoadDataStorage RoadDataStorage;
    public Team Team;
    public AttackController AttackController;
    public Road Road;
    [Header("Road")]
    public float MinRoadLength = 70;
    public float MaxRoadLength = 200;
    public float StartRoadPosition = 5;
    public float EndRoadPositionIndent = 10;
    public float RoadLengthStep = 15;
    [Header("Groups")]
    public int GroupCountDifBetweenMinMax = 5;
    public int MaxGroupCount = 20;
    public float MinGroupHeight = 1;
    public float MaxGroupHeight = 3;
    public float MinDistanceBetweenGroupBorders = 5;
    public float BonusIndentFromGroup = 1;
    public float GroupPositionTolerance = 0.1f;
    [Header("Saw Group")]
    [Min(1)] public int MinSawCountInGroup = 3;
    [Min(2)] public int MaxSawCountInGroup = 8;
    [Range(1, 8)] public float SawIndent = 2;
    [Min(5)] public float SawGroupIndent = 10;
    [Header("Budget")]
    public float MinBudget = 25;
    public float BudgetStep = 15;
    public float MinGroupBudget = 3;
    [Header("Chances")] 
    [Range(0, 100)] public float ChanceToGenerateWeaponBox = 40;
    [Range(0, 100)] public float GemChance = 40;
    [Range(0, 100)] public float ChanceForSawGroup = 30;
    [System.NonSerialized]
    public float PivotEnemyHp;
    [System.NonSerialized]
    public float GiantHp;
    [System.NonSerialized]
    public float GiantArmoredHp;
    const int MIN_OBSTACLE_TRACK_INDENT = 2; // a group of 10 amoguses can bypass
    const int TRACKS = 9;
    Dictionary<Difficulty, float> difficultFactor = new Dictionary<Difficulty, float>() {
        { Difficulty.Noob, 0.8f },
        { Difficulty.Pro, 1f },
        { Difficulty.Hacker, 1.3f },
    };
    Dictionary<Difficulty, int> groupPositionsToSkip = new Dictionary<Difficulty, int>() {
        { Difficulty.Noob, 3 },
        { Difficulty.Pro, 2 },
        { Difficulty.Hacker, 1 },
    };
    Dictionary<Difficulty, int> extraTeamCountForGiant = new Dictionary<Difficulty, int>() {
        { Difficulty.Noob, 2 },
        { Difficulty.Pro, 1 },
        { Difficulty.Hacker, 0 },
    };
    Dictionary<Difficulty, int> extraTeamCountForArmoredGiant = new Dictionary<Difficulty, int>() {
        { Difficulty.Noob, 1 },
        { Difficulty.Pro, 1 },
        { Difficulty.Hacker, 0 },
    };
    Dictionary<RoadObjectType, float> enemyWeights = new Dictionary<RoadObjectType, float>() {
        { RoadObjectType.EnemySimple, 1 },
        { RoadObjectType.EnemyGiant, 5 },
        { RoadObjectType.EnemyGiantArmored, 13 }
    };
    Dictionary<Difficulty, float> totalRewardByDifficulty = new Dictionary<Difficulty, float>() {
        { Difficulty.Noob, 100 },
        { Difficulty.Pro, 155 },
        { Difficulty.Hacker, 185 },
    };
    Dictionary<Difficulty, float> sawIndentByDifficulty = new Dictionary<Difficulty, float>() {
        { Difficulty.Noob, 1 },
        { Difficulty.Pro, 0 },
        { Difficulty.Hacker, -1 },
    };
    public Dictionary<RoadObjectType, float> EnemyCoinReward = new Dictionary<RoadObjectType, float>();
    public RoadDataViewModel GetLevelViewModel(int levelNo, Difficulty difficulty) {
        RoadDataViewModel vm = RoadDataStorage.Levels.FirstOrDefault(l => l.LevelNo == levelNo);
        if (vm != null) {
            float budget = CalcBudgetForPredefinedLevel(vm);
            CalcCoinRewards(budget, difficulty);
        }
        vm = vm == null ? GenerateLevelData(levelNo, difficulty) : vm;
        System.Array.Sort(vm.Objects, (RoadObjectViewModel a, RoadObjectViewModel b) => { return a.Position.CompareTo(b.Position); });
        return vm;
    }
    RoadDataViewModel GenerateLevelData(int levelNo, Difficulty difficulty) {
        RoadDataViewModel vm = new RoadDataViewModel() { LevelNo = levelNo };
        vm.Length = GetRoadLength(levelNo);
        GroupRectSaw sawGroup = null;
        if (Random.Range(0, 100) < ChanceForSawGroup) {
            SawIndent += sawIndentByDifficulty[difficulty];
            sawGroup = GetSawGroup(0);
        }
        vm.Objects = GetRoadObjects(levelNo, difficulty, vm.Length, sawGroup).ToArray();
        if (sawGroup != null) vm.Length += sawGroup.MaxPosition - sawGroup.MinPosition;
        return vm;
    }
    List<RoadObjectViewModel> GetRoadObjects(int levelNo, Difficulty difficulty, float length, GroupRectSaw sawGroup) {
        float budget = GetBudget(levelNo, difficulty);
        CalcCoinRewards(budget, difficulty);
        Random.InitState(System.DateTime.Now.Millisecond);
        return SpendBudget(budget, length, difficulty, sawGroup);
    }
    float CalcBudgetForPredefinedLevel(RoadDataViewModel vm) {
        float budget = 0;
        foreach (RoadObjectViewModel ovm in vm.Objects) {
            switch (ovm.RoadObjectType) {
                case RoadObjectType.EnemySimple: budget += enemyWeights[RoadObjectType.EnemySimple]; break;
                case RoadObjectType.EnemyGiant: budget += enemyWeights[RoadObjectType.EnemyGiant]; break;
                case RoadObjectType.EnemyGiantArmored: budget += enemyWeights[RoadObjectType.EnemyGiantArmored]; break;
                default: break;
            }
        }
        return budget;
    }
    void CalcCoinRewards(float budget, Difficulty difficulty) {
        float totalReward = totalRewardByDifficulty[difficulty];
        float coinByBudget = totalReward / budget;
        EnemyCoinReward.Add(RoadObjectType.EnemySimple, coinByBudget * enemyWeights[RoadObjectType.EnemySimple]);
        EnemyCoinReward.Add(RoadObjectType.EnemyGiant, coinByBudget * enemyWeights[RoadObjectType.EnemyGiant]);
        EnemyCoinReward.Add(RoadObjectType.EnemyGiantArmored, coinByBudget * enemyWeights[RoadObjectType.EnemyGiantArmored]);
    }
    float GetBudget(int levelNo, Difficulty difficulty) {
        return (MinBudget + (levelNo / 3) * BudgetStep) * difficultFactor[difficulty];
    }
    float GetRoadLength(int levelNo) {
        int minGroupCount = MaxGroupCount - GroupCountDifBetweenMinMax;
        float minLength = minGroupCount * MaxGroupHeight + (minGroupCount - 1) * MinDistanceBetweenGroupBorders
            + StartRoadPosition + EndRoadPositionIndent;
        return Mathf.Max(minLength, Mathf.Min(MaxRoadLength, MinRoadLength + (levelNo / 2) * RoadLengthStep));
    }
    float CalcPositionClearDuration(float teamDamage, float pivotEnemyHp, float cooldown) {
        return cooldown * Mathf.Ceil(pivotEnemyHp / teamDamage);
    }
    float CalcMaxGroupWidth(float teamWidth, int maxPositionCount, Difficulty difficulty) {
        int positionsToCover = Mathf.Max(1, maxPositionCount - groupPositionsToSkip[difficulty]);
        return Mathf.Min(Road.Width, positionsToCover * (teamWidth - GroupPositionTolerance));
    }
    int CalcGroupCount(float budget, float length) {
        int maxGroupCount = (int)Mathf.Ceil((length +  MinDistanceBetweenGroupBorders) / (MaxGroupHeight + MinDistanceBetweenGroupBorders));
        maxGroupCount = (int)Mathf.Min(maxGroupCount, Mathf.Floor(budget / MinGroupBudget));
        int minGroupCount = maxGroupCount - GroupCountDifBetweenMinMax;
        int groupCount = Random.Range(minGroupCount, maxGroupCount);
        return groupCount;
    }
    List<RoadObjectViewModel> SpendBudget(float budget, float length, Difficulty difficulty, GroupRectSaw sawGroup) {
        float timeToEncounter = Team.AttackRange / Road.Speed;
        WeaponDefinition pivotWeapon = AttackController.weaponsStaticData.Items.First(i => i.Type == WeaponType.Rifle);
        List<RoadObjectViewModel> objects = new List<RoadObjectViewModel>();
        int groupCount = CalcGroupCount(budget, length);
        List<float> groupPositions = GetGroupPositions(groupCount, length);
        if (sawGroup != null) {
            int sawGroupIndex = Random.Range(1, groupCount - 1);
            sawGroup.Position = groupPositions[sawGroupIndex];
            for (int i = sawGroupIndex; i < groupPositions.Count; i++) {
                groupPositions[i] += sawGroup.MaxPosition;
            }
            sawGroup.MinPosition = sawGroup.Position;
            sawGroup.MaxPosition += sawGroup.MinPosition;
        }
        List<GroupRect> groupRects = new List<GroupRect>(groupCount);
        for (int i = 0; i < groupCount; i++) {
            GroupRect rect = new GroupRect() { GroupLayout = GetGroupLayout(), Position = groupPositions[i] };
            SetGroupHeight(rect);
            groupRects.Add(rect);
        }
        List<RoadObjectViewModel> bonuses = GenerateBonuses(groupRects);
        objects.AddRange(bonuses);
        if (sawGroup != null) {
            objects.AddRange(GenerateObstacles(sawGroup, bonuses));
        }
        List<float> groupBudgets = GetGroupBudgets(groupCount, budget);
        int teamCountForFirstGiant = -1;
        int teamCountForFirstArmoredGiant = -1;
        for (int i = 0; i < groupCount; i++) {
            GroupRect rect = groupRects[i];
            int cagesPassed = bonuses.Count(b => b.RoadObjectType == RoadObjectType.Cage && b.Position < rect.Position);
            int potentialTeamCount = Team.StartupCount + cagesPassed;
            float potentialDamage = AttackController.GetPotentialTeamDamage(potentialTeamCount, pivotWeapon.Damage);
            teamCountForFirstGiant = GetTeamCountToAllowEnemyInGroup(
                teamCountForFirstGiant, timeToEncounter, RoadObjectType.EnemyGiant,
                potentialDamage, GiantHp, pivotWeapon.AttackCooldown,
                extraTeamCountForGiant[difficulty], potentialTeamCount, rect
            );
            teamCountForFirstArmoredGiant = GetTeamCountToAllowEnemyInGroup(
                teamCountForFirstArmoredGiant, timeToEncounter, RoadObjectType.EnemyGiantArmored,
                potentialDamage, GiantArmoredHp, pivotWeapon.AttackCooldown,
                extraTeamCountForArmoredGiant[difficulty], potentialTeamCount, rect
            );
            float pivotHp = PivotEnemyHp;
            if (rect.PossibleEnemyTypes.Contains(RoadObjectType.EnemyGiant)) pivotHp = GiantHp;
            if (rect.PossibleEnemyTypes.Contains(RoadObjectType.EnemyGiantArmored)) pivotHp = GiantArmoredHp;
            float timeToClearPosition = CalcPositionClearDuration(potentialDamage, pivotHp, pivotWeapon.AttackCooldown);
            int maxClearedPositions = (int)Mathf.Floor(timeToEncounter / timeToClearPosition);
            float groupWidth = CalcMaxGroupWidth(Team.GetPotentialWidth(potentialTeamCount), maxClearedPositions, difficulty);
            SetGroupWidth(rect, groupWidth);
            //Debug.Log(rect.ToString());
            //Debug.Log("Budget: " + groupBudgets[i]);
            objects.AddRange(GetGroup(groupBudgets[i], rect));
        }
        //objects.AddRange(GenerateObstacles(groupRects, bonuses));
        return objects;
    }
    GroupRectSaw GetSawGroup(float position) {
        GroupRectSaw rect = new GroupRectSaw() { GroupType = GroupType.Obstacles };
        rect.SawCount = Random.Range(MinSawCountInGroup, MaxSawCountInGroup);
        float groupHeight = rect.SawCount * (SawIndent - 1) + SawGroupIndent;
        rect.Position = position;
        rect.MinPosition = position;
        rect.MaxPosition = position + groupHeight;
        return rect;
    }
    int GetTeamCountToAllowEnemyInGroup(
        int teamCountForFirstEnemy, float timeToEncounter, RoadObjectType enemyType, float dmg, float enemyHp,
        float weaponCooldown, int extraTeamCount, int potentialTeamCount, GroupRect rect
    ) {
        int teamCountForFirstEnemtyResult = teamCountForFirstEnemy;
        if (teamCountForFirstEnemy == -1) {
            float timeToClearGiant = CalcPositionClearDuration(dmg, enemyHp, weaponCooldown);
            //Debug.Log("Time To Clear: " + timeToClearGiant);
            //Debug.Log("potentialTeamCount: " + potentialTeamCount);
            if (timeToClearGiant < timeToEncounter) {
                teamCountForFirstEnemtyResult = potentialTeamCount;
                if (potentialTeamCount >= teamCountForFirstEnemtyResult + extraTeamCount) {
                    rect.PossibleEnemyTypes.Add(enemyType);
                }
            }
        } else {
            if (potentialTeamCount >= teamCountForFirstEnemy + extraTeamCount) {
                    rect.PossibleEnemyTypes.Add(enemyType);
            }
        }
        return teamCountForFirstEnemtyResult;
    }
    List<float> GetGroupPositions(int groupCount, float length) {
        float avgDistanceBetweenGroups = (length - StartRoadPosition - EndRoadPositionIndent) / (groupCount - 1);
        float groupIndent = MinDistanceBetweenGroupBorders + MaxGroupHeight;
        float maxGroupIndent = avgDistanceBetweenGroups * 2 - groupIndent;
        float groupIndentDelta = (maxGroupIndent - groupIndent) / (groupCount - 1);
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
        float extentPercent = 0.4f; 
        float startBudget = avgGroupBudget * (1 - extentPercent);
        float endBudget = avgGroupBudget * (1 + extentPercent);
        float step = (endBudget - startBudget) / groupCount;
        List<float> groupBudgets = new List<float>(groupCount);
        for (int i = 0; i < groupCount; i++) {
            groupBudgets.Add(startBudget + i * step);
        }
        return groupBudgets;
    }
    List<RoadObjectViewModel> GetGroup(float budget, GroupRect rect) {
        List<RoadObjectViewModel> objects = new List<RoadObjectViewModel>();
        if (rect.GroupLayout == GroupLayout.Quad) {
            float budgetNorm = Mathf.Ceil(budget);
            while (budgetNorm > 0) {
                objects.Add(GetNextEnemy(budgetNorm, rect));
                budgetNorm -= enemyWeights[objects.Last().RoadObjectType];
            }
        }
        return objects;
    }
    void SetGroupHeight(GroupRect rect) {
        if (rect.GroupLayout == GroupLayout.Quad) {
            float height = Random.Range(MinGroupHeight, MaxGroupHeight);
            rect.MinPosition = rect.Position - height / 2;
            rect.MaxPosition = rect.Position + height / 2;
        }
    }
    void SetGroupWidth(GroupRect rect, float maxGroupWidth) {
        float trackWidth = Road.Width / Road.tracksCount;
        int tracksToCover = (int)Mathf.Floor(maxGroupWidth / trackWidth);
        if (rect.GroupLayout == GroupLayout.Quad) {
            int startTrackNo = Random.Range(0, TRACKS - tracksToCover);
            bool isLeft = Random.Range(0, 2) == 0;
            if ((isLeft ? startTrackNo + tracksToCover : TRACKS - startTrackNo) == 10) {
                object debug = new object();
            }
            rect.MinTrackNo = isLeft ? startTrackNo : TRACKS - startTrackNo - tracksToCover;
            rect.MaxTrackNo = isLeft ? startTrackNo + tracksToCover : TRACKS - startTrackNo;
            if (rect.MinTrackNo == rect.MaxTrackNo) {
                object debug = new object();
            }
        }
    }
    GroupLayout GetGroupLayout() {
        return GroupLayout.Quad;
        // GroupLayout[] layouts = System.Enum.GetValues(typeof(GroupLayout)).Cast<GroupLayout>().ToArray();
        // return layouts[UnityEngine.Random.Range(0, layouts.Length)];
    }
    RoadObjectViewModel GetNextEnemy(float budgetLeft, GroupRect rect) {
        List<RoadObjectType> availableTypes = rect.PossibleEnemyTypes.Where(t => enemyWeights[t] <= budgetLeft).ToList();
        RoadObjectType objectType = availableTypes[Random.Range(0, availableTypes.Count)];
        float position = Random.Range(rect.MinPosition, rect.MaxPosition);
        int trackNo = Random.Range(rect.MinTrackNo, rect.MaxTrackNo + 1);
        return new RoadObjectViewModel() {
            RoadObjectType = objectType,
            Position = position,
            TrackNo = trackNo
        };
    }
    List<RoadObjectViewModel> GenerateObstacles(GroupRectSaw sawGroup, List<RoadObjectViewModel> bonuses) {
        List<RoadObjectViewModel> objects = new List<RoadObjectViewModel>();
        const float trackPartByTeamMember = (TRACKS - MIN_OBSTACLE_TRACK_INDENT) / Team.MAX_CAPACITY;
        int cagesPassed = bonuses.Count(b => b.RoadObjectType == RoadObjectType.Cage && b.Position < sawGroup.MinPosition);
        int potentialTeamCount = Team.StartupCount + cagesPassed;
        int maxIndent = MIN_OBSTACLE_TRACK_INDENT + (int)Mathf.Floor((Team.MAX_CAPACITY - potentialTeamCount) * trackPartByTeamMember) - 1;
        for (int i = 0; i < sawGroup.SawCount; i++) {
            float position = sawGroup.MinPosition + i * SawIndent;
            bool isLeft = Random.Range(0, 2) == 1;
            int trackNo = isLeft ? maxIndent : TRACKS - maxIndent;
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
        bool shouldGenerateGem = Random.Range(0, 100) < GemChance;
        List<RoadObjectViewModel> objects = new List<RoadObjectViewModel>();
        int maxCages = Team.MAX_CAPACITY - Team.StartupCount;
        int cagesCount = 0;
        for (int i = 0; i < groupRects.Count - 1; i++) {
            bool skipGeneration = false;
            List<RoadObjectType> bonusTypes = new List<RoadObjectType>() { RoadObjectType.WeaponBox };
            if (cagesCount < maxCages)
                bonusTypes.Add(RoadObjectType.Cage);
            if (shouldGenerateGem)
                bonusTypes.Add(RoadObjectType.Gem);
            RoadObjectType bonusType = bonusTypes[Random.Range(0, bonusTypes.Count)];
            switch (bonusType) {
                case RoadObjectType.Cage: cagesCount++; break;
                case RoadObjectType.WeaponBox: skipGeneration = Random.Range(0, 100) > ChanceToGenerateWeaponBox; break;
                case RoadObjectType.Gem: shouldGenerateGem = false; break;
            }
            if (!skipGeneration) {
                RoadObjectViewModel ovm = new RoadObjectViewModel() {
                    RoadObjectType = bonusType,
                    TrackNo = Random.Range(0, TRACKS)
                };
                //bool isInside = Random.Range(0, 2) == 0;
                bool isInside = false;
                ovm.Position = isInside ? groupRects[i].GetRandomPosition()
                    : Random.Range(groupRects[i].MaxPosition + BonusIndentFromGroup, groupRects[i + 1].MinPosition - BonusIndentFromGroup);
                objects.Add(ovm);
            }
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
    EnemySimple, EnemyGiant, EnemyGiantArmored, ObstacleSaw, Cage, WeaponBox, Gem
}
public enum GroupLayout {
    Quad, Line, ZigZag
}
public enum GroupType {
    Enemies, Obstacles
}
public class GroupRect {
    public int MinTrackNo, MaxTrackNo;
    public float MinPosition, MaxPosition;
    public float Position;
    public GroupLayout GroupLayout;
    public GroupType GroupType = GroupType.Enemies;
    public List<RoadObjectType> PossibleEnemyTypes = new List<RoadObjectType>() { RoadObjectType.EnemySimple };
    public float GetRandomPosition() {
        return Random.Range(MinPosition, MaxPosition);
    }
    public override string ToString() {
        return string.Format("GroupRect. Positions: {0} - {1}. Tracks: {2}-{3}. Types: {4}", MinPosition, MaxPosition, MinTrackNo, MaxTrackNo, PossibleEnemyTypes.Count.ToString());
    }
}
public class GroupRectSaw: GroupRect {
    public int SawCount;
}