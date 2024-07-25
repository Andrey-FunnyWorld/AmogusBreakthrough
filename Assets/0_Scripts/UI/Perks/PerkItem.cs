using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PerkItem : MonoBehaviour {
    public Image PerkImage;
    public TextMeshProUGUI PerkName, PerkDescription;
    public AlphaChanger AlphaChanger;
    public RectTransform RectTransform;
    PerkModel perkModel;
    public void Init(PerkModel model) {
        perkModel = model;
        PerkImage.sprite = model.Sprite;
        PerkName.text = model.Name;
        PerkDescription.text = model.Description;
        SetTextVisibility(false);
        RectTransform = GetComponent<RectTransform>();
    }
    public void SetTextVisibility(bool show) {
        PerkName.gameObject.SetActive(show);
        PerkDescription.gameObject.SetActive(show);
    }
    public PerkType PerkType {
        get { return perkModel.PerkType; }
    }
    public float Alpha {
        get { return AlphaChanger.Alpha; }
        set { AlphaChanger.SetAlpha(value); }
    }
}

public enum PerkType {
    ExtraGuy, BossDamage, ExtraHealth, ExtraAttackWidth,
    SlowWalkSpeed,
    AttackZoneVisibility,
    RegenHP,
    WeaponBoxTransparency,
    OnePunchKill,
    Bubble,
    ExtraCoins,
    ExtraHealthUltra
}