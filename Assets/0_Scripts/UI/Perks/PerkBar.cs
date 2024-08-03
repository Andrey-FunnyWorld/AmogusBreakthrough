using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PerkBar : MonoBehaviour {
    public float AnimationDuration = 0.7f;
    public GridLayoutGroup PerkContainer;
    public void AcceptPerk(PerkItem perkItem) {
        Image image = Instantiate(perkItem.PerkImage, transform.parent);
        Vector2 startPos = (Vector2)perkItem.transform.position;
        int index = PerkContainer.transform.childCount;
        Vector2 endPos = (Vector2)PerkContainer.transform.position + new Vector2((index + 0.5f) * PerkContainer.cellSize.x, PerkContainer.cellSize.y / 2);
        Vector2 startSize = image.rectTransform.sizeDelta;
        Vector2 endSize = PerkContainer.cellSize;
        StartCoroutine(MoveItem(image.rectTransform, startPos, endPos, startSize, endSize));
    }
    IEnumerator MoveItem(RectTransform perkItem, Vector2 startPos, Vector2 endPos, Vector2 startSize, Vector2 endSize) {
        float timer = 0;
        while (timer < AnimationDuration) {
            timer += Time.deltaTime;
            float ratio = timer / AnimationDuration;
            perkItem.position = Vector2.Lerp(startPos, endPos, ratio);
            perkItem.sizeDelta = Vector2.Lerp(startSize, endSize, ratio);
            yield return null;
        }
        perkItem.SetParent(PerkContainer.transform);
    }
}
