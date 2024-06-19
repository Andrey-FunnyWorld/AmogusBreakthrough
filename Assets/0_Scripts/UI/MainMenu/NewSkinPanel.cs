using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NewSkinPanel : MonoBehaviour {
    public Image ColorImage;
    public Image SkinItemImage;
    public Animator Animator;
    public SkinQualityColors QualityColors;
    ShopItemModel item;
    SkinType shopType;
    public void ShowItem(ShopItemModel itemModel, SkinType shopType) {
        ColorImage.color = QualityColors.Colors.First(c => c.Quality == itemModel.Quality).Color;
        SkinItemImage.sprite = itemModel.Sprite;
        gameObject.SetActive(true);
        Animator.SetBool("Show", true);
        item = itemModel;
        this.shopType = shopType;
        EventManager.TriggerEvent(EventNames.ShopItemPurchaseTry, new ShopItemPurchaseArgs() { ForFree = true, ItemModel = itemModel });
    }
    public void Hide() {
        Animator.SetBool("Show", false);
    }
    public void HideEvent() {
        gameObject.SetActive(false);
    }
    public void Equip() {
        SkinItemEquipArgs args = new SkinItemEquipArgs() {
            ItemModel = item,
            ShopType = shopType,
        };
        EventManager.TriggerEvent(EventNames.SkinItemEquip, args);
    }
}
