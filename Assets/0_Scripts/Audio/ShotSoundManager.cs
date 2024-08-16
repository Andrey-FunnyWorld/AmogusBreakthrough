using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShotSoundManager : MonoBehaviour {
    public int StartAudioSourceCount = 5;
    public AudioSource OriginalSource;
    public float PitchExtent = 0.3f;
    [SerializeField] private AudioClip rifle;
    [SerializeField] private AudioClip blaster;
    [SerializeField] private AudioClip bazooka;
    [SerializeField] private AudioClip ionGun;
    [SerializeField] private float riflePause;
    [SerializeField] private float blasterPause;
    [SerializeField] private float bazookaPause;
    [SerializeField] private float ionGunPause;
    List<AudioSource> srcs;
    bool isPlaying = false;
    WeaponType WeaponType;
    Dictionary<WeaponType, float> weaponPauses;
    List<Coroutine> soundRoutines;
    void Start() {
        srcs = new List<AudioSource>(StartAudioSourceCount);
        soundRoutines = new List<Coroutine>(StartAudioSourceCount) { null };
        for (int i = 0; i < StartAudioSourceCount - 1; i++) {
            AudioSource src = Instantiate(OriginalSource, transform);
            src.pitch = Random.Range(1 - PitchExtent, 1 + PitchExtent);
            srcs.Add(src);
            soundRoutines.Add(null);
        }
        srcs.Add(OriginalSource);
        OriginalSource.pitch = Random.Range(1 - PitchExtent, 1 + PitchExtent);
        weaponPauses = new Dictionary<WeaponType, float>() {
            { WeaponType.Rifle, riflePause },
            { WeaponType.Blaster, blasterPause },
            { WeaponType.Bazooka, bazookaPause },
            { WeaponType.IonGun, ionGunPause }
        };
    }
    public void Play() {
        if (!isPlaying) {
            isPlaying = true;
            srcs[0].Play();
            for (int i = 0; i < srcs.Count; i++ ) {
                soundRoutines[i] = StartCoroutine(SoundPause(srcs[i], i));
            }
        }
    }
    IEnumerator SoundPause(AudioSource src, int index) {
        while (src.isPlaying) {
            yield return null;
        }
        float timer = 0;
        float time = Random.Range(0.05f, weaponPauses[WeaponType]);
        while (timer < time) {
            timer += Time.deltaTime;
            yield return null;
        }
        src.Play();
        soundRoutines[index] = StartCoroutine(SoundPause(srcs[index], index));
    }
    public void SetWeapon(WeaponType weaponType) {
        WeaponType = weaponType;
        AudioClip clip = null;
        switch (weaponType) {
            case WeaponType.Rifle: clip = rifle; break;
            case WeaponType.Bazooka: clip = bazooka; break;
            case WeaponType.Blaster: clip = blaster; break;
            case WeaponType.IonGun: clip = ionGun; break;
        }
        foreach (AudioSource src in srcs) {
            src.Stop();
            src.clip = clip;
            if (isPlaying)
                Play();
        }
    }
    public void Stop() {
        if (isPlaying) {
            for (int i = 0; i < srcs.Count; i++ ) {
                //srcs[i].Stop();
                if (soundRoutines[i] != null)
                    StopCoroutine(soundRoutines[i]);
            }
            isPlaying = false;
        }
    }
}

public class AudioSourceState {

}