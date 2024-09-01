using UnityEngine;

public class Weapon : Attackable {
    
    [SerializeField] private WeaponBoxMarker boxMarker;
    [SerializeField] private GameObject highlightPoint;
    public WeaponMarker weaponMarker;
    public WeaponType WeaponType;
    public Renderer BoxRenderer;
    public Transform VisiblePerkPointPosition;

    Vector3 initialWeaponPosition;
    bool shouldFallWeaponDown;

    int interpolationFramesCount = 360;
    int elapsedFrames = 0;

    protected override void Init() {
        base.Init();
        initialWeaponPosition = weaponMarker.transform.localPosition;
    }

    void Update() {
        if (shouldFallWeaponDown)
            AnimateFallDown();
    }

    public override void Destroyed() {
        base.Destroyed();
        boxMarker.gameObject.SetActive(false);
        HealthBar.gameObject.SetActive(false);
        highlightPoint.SetActive(false);
        weaponMarker.gameObject.SetActive(true);
        if (weaponMarker.transform.localPosition != initialWeaponPosition)
            SetShouldAnimateFallDown();
    }

    public void MakeTransparent() {
        weaponMarker.gameObject.SetActive(true);
        weaponMarker.transform.localPosition = VisiblePerkPointPosition.localPosition;
        highlightPoint.SetActive(true);
    }

    public void OnPickedUp() {
        EventManager.TriggerEvent(EventNames.WeaponChanged, WeaponType);
        Destroy(gameObject);
    }

    public void TurnOffDieFx() =>
        base.TurnOffDieFx();

    void SetShouldAnimateFallDown() {
        elapsedFrames = 0;
        shouldFallWeaponDown = true;
    }

    void AnimateFallDown() {
        float interpolationRatio = (float)elapsedFrames / interpolationFramesCount;
        Vector3 interpolatedPosition = Vector3.Lerp(weaponMarker.transform.localPosition, initialWeaponPosition, interpolationRatio);
        elapsedFrames = (elapsedFrames + 1) % (interpolationFramesCount + 1);
        weaponMarker.transform.localPosition = interpolatedPosition;
        if (elapsedFrames == interpolationFramesCount) {
            elapsedFrames = 0;
            shouldFallWeaponDown = false;
        }
    }
}
