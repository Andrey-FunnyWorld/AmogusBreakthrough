using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Wheel : MonoBehaviour {
    public WheelItem ItemOnWheelPrefab;
    public Transform WheelTransform;
    public float StartSpeed = 1440;
    bool isSpinning = false;
    const float DURATION_EXTENT = 1;
    const int SECTOR_COUNT = 8;
    const int RARE_SECTOR_COUNT = 2;
    const int EPIC_SECTOR_COUNT = 1;
    public void Spin(float duration) {
        if (!isSpinning) {
            isSpinning = true;
            StartCoroutine(Spinning(GetDuration(duration)));
        }
    }
    float GetDuration(float baseDuration) {
        return Random.Range(baseDuration - DURATION_EXTENT, baseDuration + DURATION_EXTENT);
    }
    IEnumerator Spinning(float duration) {
        float timer = 0;
        while (timer < duration) {
            timer += Time.deltaTime;
            float speed = StartSpeed * Utils.EaseInSquare(1 - timer / duration);
            WheelTransform.Rotate(0, 0, -speed / 360);
            yield return null;
        }
        isSpinning = false;
    }
    public void GenerateItems() {
        float radius = 0.7f * WheelTransform.GetComponent<RectTransform>().sizeDelta.x / 2;
        Debug.Log(radius);
        for (int i = 0; i < SECTOR_COUNT; i++) {
            WheelItem item = Instantiate(ItemOnWheelPrefab, Vector2.zero, Quaternion.identity, WheelTransform);
            float deg = (i + 0.5f) * 360 / SECTOR_COUNT;
            if (i == 7) item.SetType(ItemType.Epic);
            else if (i == 6 || i == 0) item.SetType(ItemType.Rare);
            else item.SetType(ItemType.Regular);
            
            item.transform.localPosition = new Vector2(radius * Mathf.Cos(deg * Mathf.Deg2Rad), radius * Mathf.Sin(deg * Mathf.Deg2Rad));
            item.transform.Rotate(new Vector3(0, 0, deg));
            //item.rot(Vector3.forward, WheelTransform.position, i * 360 / SECTOR_COUNT);
        }
    }
    void Start() {
        GenerateItems();
    }
}

public enum ItemType {
    Regular, Rare, Epic
}