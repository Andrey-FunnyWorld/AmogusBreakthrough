using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PerkStorage", menuName = "ScriptableObjects/PerkStorage", order = 1)]
public class PerkStorage : ScriptableObject {
    public PerkModel[] Perks;
}

[Serializable]
public class PerkModel {
    public PerkType PerkType;
    public string Name { get {
        return MyLocalization.Instance.GetLocalizedText(
            MyLocalization.GetPerkNameKey(PerkType)
        );
    }}
    public string Description { get {
        return MyLocalization.Instance.GetLocalizedText(
            MyLocalization.GetPerkDescriptionKey(PerkType)
        );
    }}
    public Sprite Sprite;
    public int Price;
}