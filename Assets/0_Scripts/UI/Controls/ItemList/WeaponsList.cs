using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapons", menuName = "ScriptableObjects/WeaponItems")]
public class WeaponsList : ScriptableObject {
    public WeaponDefinition[] Items;
}

public enum WeaponType {
    MachineGun, RocketLauncher
}

[Serializable]
public class WeaponDefinition {
    public WeaponType Type;
    public float Damage;
}
