using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonDisabled : MonoBehaviour {
    public Image Image;
    //public Sprite DisabledSprite;

    Button button;
    Color enabledColor;
    bool enable = true;
    //Sprite enabledSprite;
    //Image btnImage;

    public bool Enable {
        get { return enable; }
        set {
            if (enable != value) {
                SetEnabled(value);
            }
        }
    }
    
    void SetEnabled(bool enabled) {
        if (button == null) Init();
        enable = enabled;
        button.enabled = enabled;
        Image.color = enabled ? enabledColor : Color.gray;
        //btnImage.sprite = enabled ? enabledSprite : DisabledSprite;
    }

    void Init() {
        button = GetComponent<Button>();
        enabledColor = Image.color;
        //btnImage = GetComponent<Image>();
        //enabledSprite = btnImage.sprite;
    }
}
