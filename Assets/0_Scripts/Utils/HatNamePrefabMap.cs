using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HatNamePrefabMap", menuName = "ScriptableObjects/HatNamePrefabMap", order = 1)]
public class HatNamePrefabMap : ScriptableObject {
    public HatSkinItem[] Hats;
}

[Serializable]
public class HatSkinItem {
    public SkinItemName SkinItemName;
    public Transform HatPrefab;
}
