using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ExtraRewardWheel : MonoBehaviour {
    public bool RunOnStart = false;
    public int SectorCount = 5;
    public float LoopDuration = 1;
    public float RewardScaleDuration = 0.3f;
    public float RewardExtraScale = 0.5f;
    public Transform Arrow;
    public Transform[] SectorContent;
    public UnityEvent RewardCallback, FailRewardCallback, BetweenHitAndRewardCallback;
    public ExtraRewardWheelPrizeBase Prizes;
    float angle = 0;
    bool canHitTest = false;
    Coroutine spinCoroutine;
    void OnEnable() {
        if (RunOnStart)
            StartRolling();
    }
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            HitTest();
        }
    }
    public void StartRolling() {
        Arrow.localRotation = Quaternion.Euler(0, 0, 90);
        spinCoroutine = StartCoroutine(Spinning());
        canHitTest = true;
    }
    public void HitTest() {
        if (canHitTest) {
            canHitTest = false;
            StopCoroutine(spinCoroutine);
            float sectorAngle = Mathf.Floor(180 / SectorCount);
            int targetSectorNo = (int)Mathf.Floor((90 - angle) / sectorAngle);
            targetSectorNo = Mathf.Min(SectorCount, targetSectorNo);
            SectorReward(targetSectorNo);
        }
    }
    void SectorReward(int sectorNo) {
        StartCoroutine(Utils.AnimateScale(RewardScaleDuration, SectorContent[sectorNo], RewardExtraScale, true));
        BetweenHitAndRewardCallback.Invoke();
        StartCoroutine(Utils.WaitAndDo(RewardScaleDuration, () => {
            ShowReward(sectorNo);
        }));
    }
    void ShowReward(int sectorNo) {
        #if UNITY_STANDALONE || UNITY_EDITOR || UNITY_EDITOR_WIN
            RewardCallback.Invoke();
            Prizes.GiveReward(sectorNo);
        #endif
        #if UNITY_WEBGL && !UNITY_EDITOR
            HtmlBridge.Instance.ShowRewarded(() => {
                RewardCallback.Invoke();
                Prizes.GiveReward(sectorNo);
            }, () => {
                FailRewardCallback.Invoke();
            });
        #endif
    }
    IEnumerator Spinning() {
        float timer = 0;
        int direction = 1;
        while (true) {
            timer += Time.deltaTime;
            float ratio = timer / LoopDuration;
            angle = direction * 90 - direction * ratio * 180;
            Arrow.localRotation = Quaternion.Euler(0, 0, angle);
            if (timer >= LoopDuration) {
                timer = 0;
                direction = -1 * direction;
            }
            yield return null;
        }
    }
}