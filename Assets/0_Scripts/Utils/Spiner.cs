using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiner : MonoBehaviour {
    public bool IsSpinning = true;
    public float Speed = 1;
    public Axis Axis = Axis.Z;
    void Update() {
        if (IsSpinning) {
            Vector3 direction = Vector3.forward;
            switch (Axis) {
                case Axis.X: direction = Vector3.right; break;
                case Axis.Y: direction = Vector3.up; break;
                case Axis.Z: direction = Vector3.forward; break;
            }
            transform.Rotate(direction * Speed * Time.deltaTime);
        }
    }
}
