using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoadSkinStorage", menuName = "ScriptableObjects/RoadSkinStorage", order = 3)]
public class RoadSkin : ScriptableObject {
    public RoadSkinData[] Skins;
}

[System.Serializable]
public class RoadSkinData {
    public Material RoadMaterial, WallMaterial;
}