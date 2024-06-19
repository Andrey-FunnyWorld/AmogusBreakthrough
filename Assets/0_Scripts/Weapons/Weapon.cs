using UnityEngine;

public class Weapon : Attackable
{
    public WeaponType WeaponType;

    [SerializeField] private WeaponMarker weaponMarker;
    [SerializeField] private WeaponBoxMarker boxMarker;

    public override void Destroyed()
    {
        boxMarker.gameObject.SetActive(false);
        weaponMarker.gameObject.SetActive(true);
    }

    public void OnPickedUp() {
        EventManager.TriggerEvent(EventNames.WeaponChanged, WeaponType);
        Destroy(gameObject);
    }
}
