using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftBoxButton : MonoBehaviour {
    public Transform GiftParent;
    public GiftBox GiftBoxPrefab;
    public MyDialog Dialog;
    public GiftController GiftController;
    public float ShowDelay = 1.4f;
    public float ShowDuration = 0.5f;
    RectTransform rectTransform;
    float startX;
    public void ShowDialog() {
        Dialog.Show(() => {
            #if UNITY_STANDALONE || UNITY_EDITOR || UNITY_EDITOR_WIN
            DropGift();
            #endif
            #if UNITY_WEBGL && !UNITY_EDITOR
            HtmlBridge.Instance.ShowRewarded(() => {
                DropGift();
            }, () => {});
            #endif
        });
    }
    public void DropGift() {
        gameObject.SetActive(false);
        GiftBox gift = Instantiate(GiftBoxPrefab, GiftParent);
        gift.GiftController = GiftController;
    }
    void Start() {
        rectTransform = GetComponent<RectTransform>();
        startX = rectTransform.anchoredPosition.x;
        rectTransform.anchoredPosition -= new Vector2(120, 0);
        StartCoroutine(AnimateShow());
    }
    IEnumerator AnimateShow() {
        float timer = 0;
        while (timer < ShowDelay) {
            timer += Time.deltaTime;
            yield return null;
        }
        Vector2 start = rectTransform.anchoredPosition;
        Vector2 end = new Vector2(startX, rectTransform.anchoredPosition.y);
        timer = 0;
        while (timer < ShowDuration) {
            timer += Time.deltaTime;
            Vector2 pos = Vector2.Lerp(start, end, timer / ShowDuration);
            rectTransform.anchoredPosition = pos;
            yield return null;
        }
    }
    void OnDisable() {
        rectTransform.anchoredPosition = new Vector2(startX, rectTransform.anchoredPosition.y);
    }
}
