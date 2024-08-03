using UnityEngine;

public class Weapon : Attackable {
    
    [SerializeField] private WeaponBoxMarker boxMarker;
    public WeaponMarker weaponMarker;
    public WeaponType WeaponType;
    public Renderer BoxRenderer;

    public override void Destroyed() {
        base.Destroyed();
        boxMarker.gameObject.SetActive(false);
        weaponMarker.gameObject.SetActive(true);
    }

    public void OnPickedUp() {
        EventManager.TriggerEvent(EventNames.WeaponChanged, WeaponType);
        Destroy(gameObject);
    }

    public void TurnOffDieFx() =>
        base.TurnOffDieFx();
}
