using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressText3D : MonoBehaviour {
    public SkinType TextType;
    public TextMeshPro Text;
    public Transform ProgressBar;
    public float MaxSize;
    public void SetProgress(int percent) {
        string text = TextType == SkinType.Hat ? LocalizationKeys.HatsProgress : LocalizationKeys.BackpackProgress;
        Text.text = string.Format(MyLocalization.Instance.GetLocalizedText(text), percent);
        float progressSize = MaxSize * percent / 100;
        Vector3 scale = new Vector3(progressSize, ProgressBar.localScale.y, ProgressBar.localScale.z);
        ProgressBar.localScale = scale;
    }
}