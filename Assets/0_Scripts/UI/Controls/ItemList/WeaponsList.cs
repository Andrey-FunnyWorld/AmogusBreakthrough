using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapons", menuName = "ScriptableObjects/WeaponItems")]
public class WeaponsList : ScriptableObject {
    public WeaponDefinition[] Items;
}

public enum WeaponType {
    Rifle, Blaster, Bazooka, IonGun
}

public enum ImpactType {
    Direct, Explosion
}

[Serializable]
public class WeaponDefinition {
    public WeaponType Type;
    public ImpactType ImpactType;
    public float Damage;
    public GameObject FxSpawner;
    public float AttackCooldown;
    [Range(1, 10)]
    public int FxEmissionFrequency;
}
