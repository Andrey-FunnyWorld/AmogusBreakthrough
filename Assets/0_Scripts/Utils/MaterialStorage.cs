using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialStorage : MonoBehaviour {
    public static MaterialStorage Instance;
    void Awake() {
        if (Instance == null) {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            Instance = this;
            InitMaterials();
        } else {
            Destroy(gameObject);
        }
    }
    void InitMaterials() {
        AmogusSkinMaterials.Add(SkinItemName.None, null);
        AmogusSkinMaterials.Add(SkinItemName.BackJoker, JokerMaterial);
        AmogusSkinMaterials.Add(SkinItemName.BackLightning, LightningMaterial);
        AmogusSkinMaterials.Add(SkinItemName.BackStar, StarMaterial);
    }
    public Material GetBackpackMaterial(SkinItemName skin) {
        return AmogusSkinMaterials[skin];
    }
    public Material GetColorMaterial(Color color) {
        if (!AmogusColorMaterials.ContainsKey(color)) {
            Material newMaterial = new Material(SourceColorMaterial);
            newMaterial.SetColor("_Color", color);
            AmogusColorMaterials.Add(color, newMaterial);
        }
        return AmogusColorMaterials[color];
    }
    Dictionary<Color, Material> AmogusColorMaterials = new Dictionary<Color, Material>();
    Dictionary<SkinItemName, Material> AmogusSkinMaterials = new Dictionary<SkinItemName, Material>();
    public Material SourceColorMaterial;
    #region Backpack Skins
    [Header("Backpack Skins")]
    public Material JokerMaterial;
    public Material LightningMaterial;
    public Material StarMaterial;
    #endregion
}

public enum SkinItemName {
    None, BackJoker, BackLightning, BackStar,
    HatCylinder, HatCrown, HatHearts, HatChinese, HatAnon, HatKid, HatToilerPaper, HatSheriff, HatViking, HatPirate, HatMexican, HatCatEars
}