using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyText : MonoBehaviour, IPlatformAdaptable {
    public void Adapt(PlatformType platformType) {
        gameObject.SetActive(platformType == PlatformType.Desktop);
    }
}
