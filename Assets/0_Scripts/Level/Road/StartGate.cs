using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartGate : MonoBehaviour {
    public Transform[] DesktopObjects, TouchObjects;
    public Animator MyAnimator;
    public void Open() {
        MyAnimator.SetTrigger("open");
    }
    public void AdjustToPlatform(PlatformType platformType) {
        foreach (Transform tr in DesktopObjects) tr.gameObject.SetActive(platformType == PlatformType.Desktop);
        foreach (Transform tr in TouchObjects) tr.gameObject.SetActive(platformType != PlatformType.Desktop);
    }
}
