using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AlphaChanger : MonoBehaviour {
    List<SpriteRenderer> spriteRenderers;
    List<TMP_Text> textRenderers;
    List<Image> UIimages;
    void CollectRenderers() {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>().ToList();
        UIimages = GetComponentsInChildren<Image>().ToList();
        textRenderers = GetComponentsInChildren<TMP_Text>().ToList();
        SpriteRenderer thisRenderer = GetComponent<SpriteRenderer>();
        if (thisRenderer != null)
            spriteRenderers.Add(thisRenderer);
        TMP_Text thisTextRenderer = GetComponent<TMP_Text>();
        if (thisRenderer != null)
            textRenderers.Add(thisTextRenderer);
        Image thisImage = GetComponent<Image>();
        if (thisImage != null)
            UIimages.Add(thisImage);
    }
    void Awake() {
        CollectRenderers();
        ApplyAlpha();
    }
    public void SetAlpha(float alpha) {
        if (Alpha != alpha) {
            Alpha = alpha;
            ApplyAlpha();
        }
    }
    [SerializeField]
    public float Alpha;
    void ApplyAlpha() {
        foreach (SpriteRenderer renderer in spriteRenderers)
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, Alpha);
        foreach (TMP_Text renderer in textRenderers)
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, Alpha);
        foreach (Image renderer in UIimages)
            renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, Alpha);
    }
    void OnValidate() {
        if (spriteRenderers == null)
            CollectRenderers();
        ApplyAlpha();
    }
    void OnDidApplyAnimationProperties() {
        OnValidate();
    }
}
