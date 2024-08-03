using System.Collections.Generic;
using UnityEngine;

public class WeaponSwitcher : MonoBehaviour
{
    [SerializeField] private GameObject rifle;
    [SerializeField] private GameObject blaster;
    [SerializeField] private GameObject bazooka;
    [SerializeField] private GameObject ionGun;

    List<GameObject> weapons = new();

    void Awake() {
        weapons.AddRange(new GameObject[] {
            rifle,
            blaster,
            bazooka,
            ionGun
        });
    }

    public void SwitchWeapon(WeaponType weapon) {
        if (weapon == WeaponType.Rifle) {
            EquipWeapon(rifle);
        } else if (weapon == WeaponType.Blaster) {
            EquipWeapon(blaster);
        } else if (weapon == WeaponType.Bazooka) {
            EquipWeapon(bazooka);
        } else if (weapon == WeaponType.IonGun) {
            EquipWeapon(ionGun);
        }
    }

    void EquipWeapon(GameObject weapon) {
        foreach (var item in weapons)
            item.SetActive(item == weapon);
    }

}
