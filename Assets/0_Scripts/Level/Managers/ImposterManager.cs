using System.Collections;
using System.Collections.Generic;
using UnityEditorInternal;
using UnityEngine;

public class ImposterManager : MonoBehaviour {
    public ImposterUI ImposterUI;
    public Team Team;
    public BattleCameraController CameraController;
    public DropPlatform[] DropPlatforms;
    public ImposterSkinned Imposter;
    public ResultUI ResultUI;
    List<int> finePlatformIndices = new List<int>();
    int maxSteps = 3;
    int imposterIndex = 0;
    bool imposterDetected = false;
    public void RunPredatorScene() {
        maxSteps = Mathf.Min(Team.MatesCount - 1, maxSteps);
        imposterIndex = Random.Range(0, Team.MatesCount);
        Debug.Log("imposterIndex: " + imposterIndex);
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
                int finePlatformIndex = Random.Range(0, Team.MatesCount);
                if (finePlatformIndex != imposterIndex && !finePlatformIndices.Contains(finePlatformIndex)) {
                    fineMatesCount--;
                    finePlatformIndices.Add(finePlatformIndex);
                }
            }
            for (int i = 0; i < finePlatformIndices.Count; i++) {
                DropPlatforms[finePlatformIndices[i]].SetChecked();
            }
        }
        if (ImposterUI.GetStep() == maxSteps - 1) {
            for (int i = 0; i < Team.MatesCount; i++) {
                if (!finePlatformIndices.Contains(i))
                    DropPlatforms[i].ReadyForSelection();
            }
        }
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            RunPredatorScene();
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
        imposterDetected = index == imposterIndex;
        CameraFocusPlatform(DropPlatforms[index]);
        StartCoroutine(Utils.WaitAndDo(2, () => {
            // stop drums sound
            ImposterUI.HandleDropResult(imposterDetected);
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
        StartCoroutine(Utils.WaitAndDo(1, () => {
            CameraController.Brain.m_DefaultBlend.m_Time = 10;
            CameraController.SwitchToCamera(BattleCameraType.ImposterEnd);
            ImposterUI.TransitionDuration = 9;
            ImposterUI.Transition(true);
        }));
    }
    public void ShowLevelResult() {
        ResultUIViewModel vm = new ResultUIViewModel() { 
            CoinReward = 100,
            DiamondReward = 1,
            ImposterDetected = imposterDetected
        };
        ResultUI.ShowResult(vm);
    }
}
