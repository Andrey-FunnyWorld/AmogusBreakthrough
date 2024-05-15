using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Amogus : MonoBehaviour {
    public Vector3 PositionOffset;
    public MeshRenderer Renderer;
    [NonSerialized]
    public Transform ActiveHat;
    Material colorMat;
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
            hat.SetParent(transform);
            hat.localPosition = Vector3.zero;
        }
    }
    const int MATERIAL_INDEX_BODY = 0;
    const int MATERIAL_INDEX_BACKPACK = 1;
    const int MATERIAL_INDEX_BACKPACK_SKIN = 3;
}
