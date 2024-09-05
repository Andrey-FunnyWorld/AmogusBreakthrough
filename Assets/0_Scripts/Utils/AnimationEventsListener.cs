using UnityEngine;
using UnityEngine.Events;

public class AnimationEventsListener : MonoBehaviour {
    public UnityEvent EventAttackFinish;

    public void OnAttackFinish() {
        EventAttackFinish?.Invoke();
    }
}
