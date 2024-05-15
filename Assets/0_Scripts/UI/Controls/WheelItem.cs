using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelItem : MonoBehaviour {
    public Image ColorImage;
    public WheelItemType ItemType;
    public void SetType(WheelItemType itemType) {
        ItemType = itemType;
        switch (itemType) {
            case WheelItemType.Rare: ColorImage.color = new Color(0.46f, 1, 0.125f); break;
            case WheelItemType.Epic: ColorImage.color = new Color(0.22f, 0.22f, 1); break;
            default: ColorImage.color = new Color(0.78f, 0.78f, 0.78f); break;
        }
    }
}
