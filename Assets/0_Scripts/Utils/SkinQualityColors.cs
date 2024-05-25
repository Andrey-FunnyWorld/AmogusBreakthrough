using System;
using UnityEngine;

[CreateAssetMenu(fileName = "SkinQuality", menuName = "ScriptableObjects/SkinQualityColors", order = 1)]
public class SkinQualityColors: ScriptableObject {
    public SkinQualityColor[] Colors;
}
[Serializable]
public class SkinQualityColor {
    public SkinItemQuality Quality;
    public Color Color;
}
