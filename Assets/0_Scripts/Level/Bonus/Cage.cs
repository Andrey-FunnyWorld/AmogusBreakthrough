using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage : RoadObjectBase {
    const float DESTROY_ANIMATION_LENGTH = 1;
    public override void Destroyed() {
        // add Amogus to the team
        EventManager.TriggerEvent(EventNames.CageDestroyed, this);
        StartCoroutine(Utils.WaitAndDo(DESTROY_ANIMATION_LENGTH, () => {
            Destroy(gameObject);
        }));
    }
}
