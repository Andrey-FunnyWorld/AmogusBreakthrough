using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class ImposterManager : MonoBehaviour {
    public ImposterUI ImposterUI;
    public Team Team;
    public BattleCameraController CameraController;
    public DropPlatform[] DropPlatforms;
    public ImposterSkinned Imposter;
    public ResultUI ResultUI;
    public AudioSource AudioSource;
    public AudioClip DrumClip, SuccessClip, FailClip;
    public AudioRandomize AudioRandomize;
    public LevelManager LevelManager;
    public CoinsController CoinsController;
    List<int> finePlatformIndices = new List<int>();
    int maxSteps = 3;
    int imposterIndex = 0;
    bool imposterDetected = false;
    public void RunImposterScene() {
        maxSteps = Mathf.Min(Team.MatesCount - 1, maxSteps);
        imposterIndex = Random.Range(0, Team.MatesCount);
        ImposterUI.Transition(true);
        StartCoroutine(Utils.WaitAndDo(ImposterUI.TransitionDuration + 0.1f, () => {
            ImposterUI.Init(Team);
            CameraController.SwitchToCamera(BattleCameraType.Imposter);
            for (int i = 0; i < Team.MatesCount; i++) {
                DropPlatforms[i].AssignDropObject(Team.GetMate(i).transform);
                DropPlatforms[i].Index = i;
                DropPlatforms[i].DropAction.AddListener(Dropped);
                Team.HideGuns();
            }
            for (int i = Team.MatesCount; i < 10; i++) {
                DropPlatforms[i].gameObject.SetActive(false);
            }
            StartCoroutine(Utils.WaitAndDo(0.3f, () => {
                ImposterUI.Transition(false);
                StartCoroutine(Utils.WaitAndDo(ImposterUI.TransitionDuration + 0.5f, () => {
                    ImposterUI.Intro(false); // to-do - set param
                }));
            }));
        }));
    }
    public void HitTest(bool ok) {
        if (ok) {
            int fineMatesCount = GetCheckedMates(ImposterUI.GetStep());
            while (fineMatesCount > 0) {
                List<int> availablePlatformIndices = new List<int>();
                for (int i = 0; i < Team.MatesCount; i++) {
                    if (!finePlatformIndices.Contains(i) && i != imposterIndex)
                        availablePlatformIndices.Add(i);
                }
                int finePlatformIndex = availablePlatformIndices[Random.Range(0, availablePlatformIndices.Count)];
                fineMatesCount--;
                finePlatformIndices.Add(finePlatformIndex);
            }
            for (int i = 0; i < finePlatformIndices.Count; i++) {
                DropPlatforms[finePlatformIndices[i]].SetChecked();
            }
        }
        if (ImposterUI.GetStep() >= maxSteps - 1) {
            for (int i = 0; i < Team.MatesCount; i++) {
                if (!finePlatformIndices.Contains(i))
                    DropPlatforms[i].ReadyForSelection();
            }
        }
    }
    List<Dictionary<int, int>> checkedMatesByStep = new List<Dictionary<int, int>>() {
        new Dictionary<int, int>() { { 2, 1 }, { 3, 1 }, { 4, 1 }, { 5, 2 }, { 6, 2 }, { 7, 2 }, { 8, 3 }, { 9, 3 }, { 10, 3 } },
        new Dictionary<int, int>() { { 3, 1 }, { 4, 1 }, { 5, 1 }, { 6, 2 }, { 7, 2 }, { 8, 2 }, { 9, 3 }, { 10, 3 } },
        new Dictionary<int, int>() { { 4, 1 }, { 5, 1 }, { 6, 1 }, { 7, 2 }, { 8, 2 }, { 9, 2 }, { 10, 3 } }
    };
    int GetCheckedMates(int step) {
        return checkedMatesByStep[step][Team.MatesCount];
    }
    public void Dropped(int index) {
        LevelManager.BattleMusic.Stop();
        foreach (DropPlatform platform in DropPlatforms) {
            platform.BlockSelection();
        }
        AudioSource.clip = DrumClip;
        AudioSource.loop = true;
        AudioSource.Play();
        imposterDetected = index == imposterIndex;
        HtmlBridge.Instance.ReportMetric(imposterDetected ? MetricNames.ImposterDetected : MetricNames.ImposterFailed);
        UserProgressController.Instance.ProgressState.ImposterDetectedCount++;
        CameraFocusPlatform(DropPlatforms[index]);
        StartCoroutine(Utils.WaitAndDo(2, () => {
            // stop drums sound
            ImposterUI.HandleDropResult(imposterDetected);
            AudioSource.Stop();
            AudioSource.clip = imposterDetected ? SuccessClip : FailClip;
            AudioSource.loop = false;
            AudioSource.Play();
            if (imposterDetected) {
                DropSuccess();
            } else {
                DropFailed();
            }
            StartCoroutine(Utils.WaitAndDo(1, () => {
                ResultUI.ShowImposterResult(imposterDetected);
            }));
        }));
    }
    void DropFailed() {
        ShowImposter(imposterIndex);
        StartCoroutine(Utils.WaitAndDo(0.1f, () => {
            CameraFocusPlatform(DropPlatforms[imposterIndex]);
        }));
    }
    void DropSuccess() {
        CameraController.Brain.m_DefaultBlend.m_Time = 3;
        CameraController.SwitchToCamera(BattleCameraType.Imposter);
    }
    void CameraFocusPlatform(DropPlatform platform) {
        CameraController.Platform.LookAt = platform.transform;
        CameraController.Platform.transform.position = platform.transform.position + new Vector3(1.23f, 1.39f, -1.78f);
        CameraController.SwitchToCamera(BattleCameraType.Platform);
    }
    void ShowImposter(int index) {
        Material colorMaterial = MaterialStorage.Instance.GetColorMaterial(Team.TeamColors[index]);
        Imposter.ApplyMaterials(colorMaterial);
        Amogus amogus = Team.GetMate(index);
        Transform hat = amogus.ActiveHat;
        if (hat != null) {
            SkinItemName hatSkin = (SkinItemName)UserProgressController.Instance.ProgressState.EquippedHats[index + 1];
            Imposter.ApplyHat(HatStorage.Instance.GetHat(hatSkin));
        }
        amogus.transform.Translate(0, -30, 0);
        Imposter.transform.position = DropPlatforms[index].ObjectToDrop.position;
        Imposter.Laugh();
        AudioRandomize.Play();
        StartCoroutine(Utils.WaitAndDo(1, () => {
            CameraController.Brain.m_DefaultBlend.m_Time = 10;
            CameraController.SwitchToCamera(BattleCameraType.ImposterEnd);
            ImposterUI.TransitionDuration = 9;
            ImposterUI.Transition(true);
        }));
    }
    public void ShowLevelResult() {
        AudioRandomize.Stop(false);
        ResultUIModel vm = new ResultUIModel() { 
            CoinReward = CoinsController.CasualCoins + (imposterDetected ? ImposterUI.CoinsForImposter : 0),
            DiamondReward = CoinsController.PremiumlCoins,
            ImposterDetected = imposterDetected
        };
        ResultUI.ShowResult(vm);
    }
}
