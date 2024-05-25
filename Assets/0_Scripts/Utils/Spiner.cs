using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spiner : MonoBehaviour {
    public bool IsSpinning = true;
    public float Speed = 1;
    void Update() {
        if (IsSpinning)
            transform.Rotate(Vector3.forward * Speed * Time.deltaTime);
    }
}
