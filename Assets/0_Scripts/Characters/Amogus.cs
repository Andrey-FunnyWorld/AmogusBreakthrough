using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amogus : MonoBehaviour {
    public Vector3 PositionOffset;
    public MeshRenderer Renderer;
    public void ApplyMovement(Vector3 newPosition) {
        Vector3 newPos = newPosition + PositionOffset;
        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
    }
    public void SetColor(Material material) {
        Renderer.material = material;
    }
}
