using UnityEngine;
using System;

[CreateAssetMenu(fileName = "BackpackNameMaterialMap", menuName = "ScriptableObjects/BackpackNameMaterialMap", order = 1)]
public class BackpackNameMaterialMap : ScriptableObject {
    public BackpackMaterialItem[] Backpacks;
}

[Serializable]
public class BackpackMaterialItem {
    public SkinItemName SkinItemName;
    public Material Material;
}