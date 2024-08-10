using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuTutorial : MonoBehaviour {
    public ProgressText3D HatProgress, BackpackProgress;
    public UnityEngine.UI.Image Hand;
    public ExpandButton ShopButton;
    public ShopList HatShop;
    public ButtonDisabled HatShopButton, CloseWheelButton, CloseShopButton, BattleButton;
    public RectTransform OpenWheelButton, WheelButton, DifficultyPanel;
    public Wheel Wheel;
    public Transform MainPart;
    public NewSkinPanel NewSkinPanel;

    public float HandLoopDuration = 0.5f;
    public float HandDistance = 150;
    public float AimTextDuration = 3;

    public Transform BuyText, AimText, ToBattleText;

    Coroutine handCoroutine;
    List<TutorialStep> Steps = new List<TutorialStep>();
    bool isRunning = false;
    ListItem boughtHat = null;
    bool equipped = false;
    Camera cam;
    void Start() {
        SubscriveEvents();
    }
    void Init() {
        cam = Camera.main;
        if (Steps.Count == 0) {
            Steps.Add(new TutorialStep() {
                StepAction = () => { 
                    DisableButtonsExceptFor(ShopButton.GetComponent<ButtonDisabled>());
                    AttachHand(ShopButton.transform, new Vector2(-1, 1));
                },
                CompleteCondition = () => { return ShopButton.IsOpened; }
            });
            Steps.Add(new TutorialStep() {
                StepAction = () => { 
                    StopCoroutine(handCoroutine);
                    Hand.gameObject.SetActive(false);
                    ShopButton.SetHintsVisibility(true);
                    DisableButtonsExceptFor(HatShopButton.GetComponent<ButtonDisabled>());
                    StartCoroutine(Utils.WaitAndDo(1.5f, () => {
                        AttachHand(HatShopButton.transform, Vector2.left);
                    }));
                },
                CompleteCondition = () => { return HatShop.gameObject.activeSelf; }
            });
            Steps.Add(new TutorialStep() {
                StepAction = () => { 
                    ShopButton.SetHintsVisibility(false);
                    BuyText.gameObject.SetActive(true);
                    DisableButtonsExceptFor(null);
                    StopCoroutine(handCoroutine);
                },
                CompleteCondition = () => { return boughtHat != null; }
            });
            Steps.Add(new TutorialStep() {
                StepAction = () => { 
                    AttachHand(boughtHat.transform, Vector2.right);
                    BuyText.gameObject.SetActive(false);
                },
                CompleteCondition = () => { return equipped; }
            });
            Steps.Add(new TutorialStep() {
                StepAction = () => {
                    DisableButtonsExceptFor(OpenWheelButton.GetComponent<ButtonDisabled>());
                    AttachHand(OpenWheelButton.transform, Vector2.up);
                },
                CompleteCondition = () => { return Wheel.gameObject.activeSelf; }
            });
            Steps.Add(new TutorialStep() {
                StepAction = () => {
                    DisableButtonsExceptFor(WheelButton.GetComponent<ButtonDisabled>());
                    AttachHand(WheelButton.transform, Vector2.right);
                },
                CompleteCondition = () => { return Wheel.IsSpinning; }
            });
            Steps.Add(new TutorialStep() {
                StepAction = () => {
                    StopCoroutine(handCoroutine);
                    Hand.gameObject.SetActive(false);
                },
                CompleteCondition = () => { return !Wheel.IsSpinning; }
            });
            Steps.Add(new TutorialStep() {
                StepAction = () => {
                    DisableButtonsExceptFor(CloseWheelButton);
                    AttachHand(CloseWheelButton.transform, new Vector2(1, 1));
                },
                CompleteCondition = () => { return !Wheel.gameObject.activeSelf; }
            });
            Steps.Add(new TutorialStep() {
                StepAction = () => {
                    DisableButtonsExceptFor(CloseShopButton);
                    AttachHand(CloseShopButton.transform, new Vector2(1, 0));
                },
                CompleteCondition = () => { return !HatShop.gameObject.activeSelf; }
            });
            Steps.Add(new TutorialStep() {
                StepAction = () => {
                    StopCoroutine(handCoroutine);
                    Hand.gameObject.SetActive(false);
                    AimText.gameObject.SetActive(true);
                    HatProgress.AnimateProgress(true);
                    BackpackProgress.AnimateProgress(false);
                    StartCoroutine(Utils.WaitAndDo(AimTextDuration, () => {
                        AimText.gameObject.SetActive(false);
                    }));
                },
                CompleteCondition = () => { return !AimText.gameObject.activeSelf; }
            });
            Steps.Add(new TutorialStep() {
                StepAction = () => {
                    ToBattleText.gameObject.SetActive(true);
                    HatProgress.StopAnimation();
                    BackpackProgress.StopAnimation();
                    AttachHand(BattleButton.transform, Vector2.up);
                    ButtonDisabled[] buttons = MainPart.GetComponentsInChildren<ButtonDisabled>();
                    foreach (ButtonDisabled btn in buttons) {
                        btn.Enable = true;
                    }
                    buttons = HatShop.GetComponentsInChildren<ButtonDisabled>(true);
                    foreach (ButtonDisabled btn in buttons) {
                        btn.Enable = true;
                    }
                },
                CompleteCondition = () => { return DifficultyPanel.gameObject.activeSelf; }
            });
            Steps.Add(new TutorialStep() {
                StepAction = () => {
                    ToBattleText.gameObject.SetActive(false);
                    StopCoroutine(handCoroutine);
                    Hand.gameObject.SetActive(false);
                    ButtonDisabled[] buttons = FindObjectsOfType<ButtonDisabled>();
                    foreach (ButtonDisabled btn in buttons) {
                        btn.Enable = true;
                    }
                },
                CompleteCondition = () => { return true; }
            });
        }
    }
    void AttachHand(Transform target, Vector2 direction) {
        Hand.gameObject.SetActive(true);
        if (handCoroutine != null)
            StopCoroutine(handCoroutine);
        Vector2 targetPos = Vector2.zero - new Vector2(0, ((RectTransform)target).sizeDelta.y / 2);
        Hand.rectTransform.SetParent(target, false);
        Vector2 startPos = targetPos + direction * HandDistance;
        handCoroutine = StartCoroutine(AttachHand(startPos, targetPos, HandLoopDuration));
        //Debug.Log(string.Format("Start: {0}; End: {1}", startPos, targetPos));
    }
    public void RunTutorial() {
        if (!isRunning){
            isRunning = true;
            Init();
            StartCoroutine(Tutorial());
        }
    }
    IEnumerator Tutorial() {
        for (int i = 0; i < Steps.Count; i++) {
            Steps[i].StepAction();
            while (!Steps[i].CompleteCondition()) {
                yield return null;
            }
        }
        if (handCoroutine != null)
            StopCoroutine(handCoroutine);
        Hand.gameObject.SetActive(false);
    }
    IEnumerator AttachHand(Vector2 startPos, Vector2 endPos, float loopTime) {
        float timer = 0;
        bool forward = true;
        while (true) {
            timer += Time.deltaTime;
            Vector2 pos = Vector2.Lerp(forward ? startPos : endPos, forward ? endPos : startPos, timer / loopTime);
            Hand.rectTransform.anchoredPosition = pos;
            if (timer > loopTime) {
                forward = !forward;
                timer = 0;
            }
            yield return null;
        }
    }
    void DisableButtonsExceptFor(ButtonDisabled button) {
        ButtonDisabled[] buttons = FindObjectsOfType<ButtonDisabled>();
        foreach (ButtonDisabled btn in buttons) {
            btn.Enable = btn == button;
        }
    }
    void ShopItemPurchased(object arg) {
        ListItem item = (ListItem)arg;
        boughtHat = item;
    }
    void SkinItemEquip(object arg) {
        SkinItemEquipArgs item = (SkinItemEquipArgs)arg;
        equipped = true;
    }
    void SubscriveEvents() {
        EventManager.StartListening(EventNames.ShopItemPurchased, ShopItemPurchased);
        EventManager.StartListening(EventNames.SkinItemEquip, SkinItemEquip);
    }
    void UnsubscriveEvents() {
        EventManager.StopListening(EventNames.ShopItemPurchased, ShopItemPurchased);
        EventManager.StopListening(EventNames.SkinItemEquip, SkinItemEquip);
    }
    void OnDestroy() {
        UnsubscriveEvents();
    }
}

public class TutorialStep {
    public Func<bool> CompleteCondition;
    public UnityAction StepAction;
}