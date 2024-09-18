using System;

public class GemOnRoad : RoadObjectBase {
    const float ICON_ANIMATION_DURATION = 0.5f;
    const float ICON_ELEVATE_DISTANCE = 15;
    [NonSerialized]
    public bool IsPicked = false;
    public void Pickup() {
        IsPicked = true;
        StartCoroutine(Utils.AnimateFloatUp(transform, ICON_ANIMATION_DURATION, ICON_ELEVATE_DISTANCE));
    }
}
