using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromoController : MonoBehaviour {
    public MenuCameraController Cameras;
    public ProgressText3D Hats, Stickers;
    public bool EnterOnStart = false;
    public float Duration = 4.2f;
    public float ChangeSkinEveryTime = 0.5f;
    public ShopList HatShop, StickerShop;
    public Transform[] ElementsToHide;
    void Start() {
        if (EnterOnStart) {
            EnterPromoMode();
        }
    }
    public void EnterPromoMode() {
        foreach (Transform tr in ElementsToHide)
            tr.gameObject.SetActive(false);
        Cameras.CameraPromo1.gameObject.SetActive(true);
        Hats.SetProgress(0);
        Stickers.SetProgress(0);
        StartCoroutine(Utils.WaitAndDo(2, () => {
            Cameras.EnterPromoMode();
            StartCoroutine(AnimateProgress(Duration));
        }));
    }
    IEnumerator AnimateProgress(float time) {
        float timer = 0;
        float hatTimer = 0;
        while (timer < time) {
            timer += Time.deltaTime;
            hatTimer += Time.deltaTime;
            float ratio = timer / time;
            Hats.SetProgress((int)Mathf.Floor(ratio * 100));
            Stickers.SetProgress((int)Mathf.Floor(ratio * 100));
            if (hatTimer >= ChangeSkinEveryTime) {
                hatTimer = 0;
                HatShop.RandomEquip();
                StickerShop.RandomEquip();
            }
            yield return null;
        }
    }
}
