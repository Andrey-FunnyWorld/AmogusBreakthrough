using UnityEngine;

public class RoadObjectBase : MonoBehaviour {
    public float RoadPosition;
    public bool CanBeMoved = true;
    public virtual void IsRunningChanged(bool isRunning) {}
}
