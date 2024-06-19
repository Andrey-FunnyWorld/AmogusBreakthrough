using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ProgressText3D : MonoBehaviour {
    public SkinType TextType;
    public TextMeshPro Text;
    public void SetProgress(int percent) {
        string text = TextType == SkinType.Hat ? LocalizationKeys.HatsProgress : LocalizationKeys.BackpackProgress;
        Text.text = string.Format(MyLocalization.Instance.GetLocalizedText(text), percent);
    }
}