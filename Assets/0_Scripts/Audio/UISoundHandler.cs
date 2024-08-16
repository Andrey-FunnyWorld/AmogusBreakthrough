using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISoundHandler : MonoBehaviour {
    public AudioSource AudioSource;
    public AudioClip ButtonClick;
    public float PitchExtent = 0.2f;
    Dictionary<UISoundType, AudioClip> clips = new Dictionary<UISoundType, AudioClip>();
    void PlayUISound(object arg) {
        UISoundType soundType = (UISoundType)arg;
        AudioSource.clip = clips[soundType];
        AudioSource.pitch = Random.Range(1 - PitchExtent, 1 + PitchExtent);
        AudioSource.Play();
    }
    void Start() {
        Init();
        SubscribeEvents();
    }
    void OnDestroy() {
        UnsubscribeEvents();
    }
    void SubscribeEvents() {
        EventManager.StartListening(EventNames.PlayUISound, PlayUISound);
    }
    void UnsubscribeEvents() {
        EventManager.StopListening(EventNames.PlayUISound, PlayUISound);
    }
    void Init() {
        clips.Add(UISoundType.ButtonClick, ButtonClick);
    }
}
public enum UISoundType {
    ButtonClick
}