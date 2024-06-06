using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WheelItem : MonoBehaviour {
    public Image ColorImage;
    public SkinItemQuality ItemType;
    public void SetType(SkinItemQuality itemType) {
        ItemType = itemType;
        switch (itemType) {
            case SkinItemQuality.Rare: ColorImage.color = new Color(0.46f, 1, 0.125f); break;
            case SkinItemQuality.Epic: ColorImage.color = new Color(0.22f, 0.22f, 1); break;
            default: ColorImage.color = new Color(0.78f, 0.78f, 0.78f); break;
        }
    }
}
