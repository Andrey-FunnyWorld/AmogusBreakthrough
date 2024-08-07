using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PController : MovementController {
    protected override Vector2 GetScreenPosition() {
        return Input.GetMouseButton(0) ? Input.mousePosition : new Vector2(noPoint, 0);
    }
    // protected override Vector2 GetInputDirection() {
    //     return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    // }
}
