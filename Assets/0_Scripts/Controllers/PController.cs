using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PController : MovementController {
    protected override Vector2 GetInputDirection() {
        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }
}
