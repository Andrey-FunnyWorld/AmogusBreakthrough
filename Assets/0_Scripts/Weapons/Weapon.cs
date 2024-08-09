using UnityEngine;

public class Weapon : Attackable {
    
    [SerializeField] private WeaponBoxMarker boxMarker;
    public WeaponMarker weaponMarker;
    public WeaponType WeaponType;
    public Renderer BoxRenderer;
    public Transform VisiblePerkPointPosition;

    Vector3 initialWeaponPosition;
    bool shouldInterpolateWeaponDown;

    int interpolationFramesCount = 360;
    int elapsedFrames = 0;

    void Start() {
        initialWeaponPosition = weaponMarker.transform.localPosition;
    }

    void Update() {
        if (shouldInterpolateWeaponDown) {
            float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;
            Vector3 interpolatedPosition = Vector3.Lerp(weaponMarker.transform.localPosition, initialWeaponPosition, interpolationRatio);
            elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);
            weaponMarker.transform.localPosition = interpolatedPosition;
            if (elapsedFrames == interpolationFramesCount) {
                elapsedFrames = 0;
                shouldInterpolateWeaponDown = false;
            }
        }
    }

    public override void Destroyed() {
        base.Destroyed();
        boxMarker.gameObject.SetActive(false);
        HealthBar.gameObject.SetActive(false);
        weaponMarker.gameObject.SetActive(true);
        if (weaponMarker.transform.localPosition != initialWeaponPosition)
            InterpolateDown();
    }

    public void MakeTransparent() {
        weaponMarker.gameObject.SetActive(true);
        weaponMarker.transform.localPosition = VisiblePerkPointPosition.localPosition;
    }

    public void OnPickedUp() {
        EventManager.TriggerEvent(EventNames.WeaponChanged, WeaponType);
        Destroy(gameObject);
    }

    public void TurnOffDieFx() =>
        base.TurnOffDieFx();

    void InterpolateDown() {
        elapsedFrames = 0;
        shouldInterpolateWeaponDown = true;
    }
}
