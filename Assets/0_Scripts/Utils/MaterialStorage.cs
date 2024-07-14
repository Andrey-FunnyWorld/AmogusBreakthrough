using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MaterialStorage : MonoBehaviour {
    public BackpackNameMaterialMap BackpackNameMaterialMap;
    public static MaterialStorage Instance;
    void Awake() {
        if (Instance == null) {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            Instance = this;
            //InitMaterials();
        } else {
            Destroy(gameObject);
        }
    }
    // void InitMaterials() {
    //     // AmogusSkinMaterials.Add(SkinItemName.None, null);
    //     // AmogusSkinMaterials.Add(SkinItemName.BackJoker, JokerMaterial);
    //     // AmogusSkinMaterials.Add(SkinItemName.BackLightning, LightningMaterial);
    //     // AmogusSkinMaterials.Add(SkinItemName.BackStar, StarMaterial);
    // }
    public Material GetBackpackMaterial(SkinItemName skin) {
        if (skin == SkinItemName.None) return null;
        if (!skin.ToString().Contains("Back")) Debug.LogError("BackpackMaterials can only give backpacks");
        return BackpackNameMaterialMap.Backpacks.First(b => b.SkinItemName == skin).Material;
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
    //Dictionary<SkinItemName, Material> AmogusBackpackMaterials = new Dictionary<SkinItemName, Material>();
    public Material SourceColorMaterial;
    // #region Backpack Skins
    // [Header("Backpack Skins")]
    // public Material JokerMaterial;
    // public Material LightningMaterial;
    // public Material StarMaterial;
    // #endregion
}

public enum SkinItemName {
    None, BackJoker, BackLightning, BackStar, BackHeart, BackRocket, BackSpray, BackMusic, BackSword, BackCatRap, BackBoot, BackOrange,
    BackWheelFire, BackHarry, BackAudio, BackPanda, BackTraktor, BackPlanet, BackSpiderman, BackHorse, BackCube,
    BackRoblox, BackShield, BackRacoon, BackPotion, BackMeteor, BackAtom, BackAmogus, BackCaptainShield, BackHelmet, BackSweet,
    BackUnicorn, BackFist, BackZombie, BackDragon, BackGamepad, BackAnimeGirl, BackHotdog, BackM, BackBulb, BackBanana,
    HatCylinder, HatCrown, HatHearts, HatChinese, HatAnon, HatKid, HatToilerPaper, HatSheriff, HatViking, HatPirate, HatMexican, HatCatEars,
    HatKnight, HatDoctor, HatVantus, HatBagVampire, HatBDay, HatEraphonesGreen, HatEraphonesPink, HatEraphonesPurple, HatEraphonesBlue, HatWizard,
    HatManiac, HatBaloon, HatBatman, HatTiger, HatIceCream, HatFrog, HatApple, HatPunk, HatAlchemist, HatBurger, HatScuba, HatCake,
    HatRoadCone, HatShark, HatHockeyBlue, HatHockeyRed, HatHockeyBlack, HatUnderwear
}