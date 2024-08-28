using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Palmmedia.ReportGenerator.Core.Reporting.Builders;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DesintegratorPanel : MonoBehaviour {
    public Desintegrator Desintegrator;
    public ProgressBarUITicks ProgressBar;
    public ProgressMilestone[] Milestones;
    public DesintegratorLegendItem DiamondLegend, DiamondsLegend, GiftLegend;
    public RectTransform CollectMsg;
    public DesintegratorScene Scene;
    public GiftBoxButton GiftBoxButton;
    public Button CloseButton;
    public ButtonDisabled BoomButton;
    public LoopScaler ChargeIcon;
     
    List<DesintegratorLegendItem> liveLegends = new List<DesintegratorLegendItem>();
    [NonSerialized]
    public float Progress;
    public bool CanBoom() {
        int lastAvailableMilestoneIndex = Milestones.Count(m => m.Progress < Progress) - 1;
        int lastTakenMilestoneIndex = UserProgressController.Instance.ProgressState.LastGiftedMilestone;
        return lastAvailableMilestoneIndex > lastTakenMilestoneIndex;
    }
    public void Test() {
        bool canBoom = CanBoom();
        SetTesting(true);
        Desintegrator.Test(canBoom, () => {
            if (canBoom) {
                UserProgressController.Instance.ProgressState.LastGiftedMilestone++;
                UpdateMilestones();
                HtmlBridge.Instance.SaveProgress();
                Scene.Show(() => {
                    Desintegrator.IsRunning = false;
                    GiveReward();
                });
            } else {
                Desintegrator.IsRunning = false;
                SetTesting(false);
                StartCoroutine(Utils.AnimateScale(0.5f, CollectMsg, 0.3f, true));
            }
        });
    }
    void GiveReward() {
        ProgressMilestone pm = Milestones[UserProgressController.Instance.ProgressState.LastGiftedMilestone];
        switch (pm.MilestoneType) {
            case MilestoneType.Diamond:
                UserProgressController.Instance.ProgressState.Spins += 1;
                HtmlBridge.Instance.SaveProgress();
                SetTesting(false);
                break;
            case MilestoneType.Diamonds:
                UserProgressController.Instance.ProgressState.Spins += 2;
                HtmlBridge.Instance.SaveProgress();
                SetTesting(false);
                break;
            case MilestoneType.Gift:
                StartCoroutine(Utils.WaitAndDo(1, () => {
                    CloseButton.onClick.Invoke();
                    GiftBoxButton.DropGift();
                    SetTesting(false);
                }));
                break;
        }
    }
    public void SetTesting(bool testing) {
        ButtonDisabled[] buttons = GetComponentsInChildren<ButtonDisabled>();
        foreach (ButtonDisabled btn in buttons) {
            btn.Enable = !testing;
        }
    }
    public void SetProgress(float progress) {
        Progress = progress;
        ApplyProgress(Progress);
        UpdateMilestones();
    }
    void ApplyProgress(float progress) {
        ProgressBar.Value = progress;        
    }
    void AddMilestones() {
        for (int i = 0; i < Milestones.Length; i++) {
            DesintegratorLegendItem prefab = GetLegendByType(Milestones[i].MilestoneType);
            DesintegratorLegendItem liveLegendItem = Instantiate(prefab, ProgressBar.transform);
            liveLegends.Add(liveLegendItem);
            ProgressBar.AddTick(Milestones[i].Progress, liveLegendItem);
        }
    }
    void UpdateMilestones() {
        int lastTakenMilestoneIndex = UserProgressController.Instance.ProgressState.LastGiftedMilestone;
        if (liveLegends.Count == 0) AddMilestones();
        for (int i = 0; i < Milestones.Length; i++) {
            liveLegends[i].Completed = i <= lastTakenMilestoneIndex;
        }
    }
    DesintegratorLegendItem GetLegendByType(MilestoneType mType) {
        switch (mType) {
            case MilestoneType.Diamond: return DiamondLegend;
            case MilestoneType.Diamonds: return DiamondsLegend;
            case MilestoneType.Gift: return GiftLegend;
        }
        return null;
    }
}
[Serializable]
public class ProgressMilestone {
    public float Progress;
    public MilestoneType MilestoneType;
}

public enum MilestoneType {
    Diamond, Diamonds, Gift
}