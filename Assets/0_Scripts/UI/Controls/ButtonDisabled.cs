using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonDisabled : MonoBehaviour {
    bool enable = true;
    public bool Enable {
        get { return enable; }
        set {
            if (enable != value) {
                SetEnabled(value);
            }
        }
    }
    public Sprite DisabledSprite;
    Sprite enabledSprite;
    Image btnImage;
    Button button;
    void SetEnabled(bool enabled) {
        if (btnImage == null) Init();
        enable = enabled;
        button.enabled = enabled;
        btnImage.sprite = enabled ? enabledSprite : DisabledSprite;
    }
    void Init() {
        btnImage = GetComponent<Image>();
        button = GetComponent<Button>();
        enabledSprite = btnImage.sprite;
    }
}
