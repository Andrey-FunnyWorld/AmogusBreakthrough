using System.Collections;
using UnityEngine;

public class RoadObjectBase : MonoBehaviour {
    public float RoadPosition;
    public virtual void IsRunningChanged(bool isRunning) {}
}
