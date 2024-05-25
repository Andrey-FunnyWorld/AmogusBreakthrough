using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinItems", menuName = "ScriptableObjects/SkinItems", order = 1)]
public class SkinItems: ScriptableObject {
    public ShopItemModel[] Items;
}
[Serializable]
public class ShopItemModel {
    public SkinItemName SkinName;
    public Sprite Sprite;
    public int Price;
    public SkinItemQuality Quality;
}

public enum SkinItemQuality {
    Regular, Rare, Epic
}