using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amogus : TeamMember {
    public Vector3 PositionOffset;
    public SkinnedMeshRenderer Renderer;
    public Transform HatPlaceholder;
    public Transform GunPlaceholderLeft, GunPlaceholderRight;
    [NonSerialized]
    public Transform ActiveHat;
    Material colorMat;
    public void SetGun(bool isLeft) { // add gun type
        GunPlaceholderLeft.gameObject.SetActive(isLeft);
        GunPlaceholderRight.gameObject.SetActive(!isLeft);
    }
    public void ApplyMovement(Vector3 newPosition) {
        Vector3 newPos = newPosition + PositionOffset;
        transform.position = new Vector3(newPos.x, transform.position.y, newPos.z);
    }
    public void ApplyMaterials(Material colorMaterial, Material backpackMaterial) {
        colorMat = colorMaterial;
        Material[] updatedMaterials = Renderer.materials;
        updatedMaterials[MATERIAL_INDEX_BODY] = colorMaterial;
        updatedMaterials[MATERIAL_INDEX_BACKPACK] = colorMaterial;
        updatedMaterials[MATERIAL_INDEX_BACKPACK_SKIN] = backpackMaterial != null ? backpackMaterial : colorMaterial;
        Renderer.materials = updatedMaterials;
    }
    public void ApplyBackpack(Material backpackMaterial) {
        Material[] updatedMaterials = Renderer.materials;
        updatedMaterials[MATERIAL_INDEX_BACKPACK_SKIN] = backpackMaterial != null ? backpackMaterial : colorMat;
        Renderer.materials = updatedMaterials;
    }
    public void ApplyHat(Transform hat) {
        ActiveHat = hat;
        if (hat != null) {
            hat.SetParent(HatPlaceholder);
            hat.localPosition = Vector3.zero;
            hat.rotation = Quaternion.Euler(0, HatPlaceholder.parent.rotation.eulerAngles.y, 0);
        }
    }
    void Start() {
        LookAroundAnimation();
    }
    void LookAroundAnimation() {
        StartCoroutine(Utils.WaitAndDo(
            UnityEngine.Random.Range(LOOK_AROUND_ANIMATION_MIN_DELAY, LOOK_AROUND_ANIMATION_MAX_DELAY),
            () => {
                Animator.SetTrigger("look");
                LookAroundAnimation();
            }
        ));
        
    }
    const int MATERIAL_INDEX_BODY = 0;
    const int MATERIAL_INDEX_BACKPACK = 1;
    const int MATERIAL_INDEX_BACKPACK_SKIN = 3;
    const float LOOK_AROUND_ANIMATION_MIN_DELAY = 4;
    const float LOOK_AROUND_ANIMATION_MAX_DELAY = 12;
}
