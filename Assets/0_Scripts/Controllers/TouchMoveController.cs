using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class TouchMoveController : MovementController {
    protected override Vector2 GetScreenPosition() {
        return Input.touchCount > 0 ? Input.touches[0].position : new Vector2(noPoint, 0);
    }
    // public HoldDownButton LeftButton, RightButton;
    // protected override Vector2 GetInputDirection() {
    //     Vector2 dir = LeftButton.IsDown ? Vector2.left : Vector2.zero;
    //     if (RightButton.IsDown) dir += Vector2.right;
    //     return dir;
    // }
    // Vector2 prevPt = Vector2.zero;
    // protected override Vector2 GetInputDirection() {
    //     if (Input.touchCount > 0) {
    //         Vector2 pos = Input.GetTouch(0).position;
    //         Vector2 dir = pos - prevPt;
    //         prevPt = pos;
    //         return dir;
    //     } else return Vector2.zero;
    // }
}
