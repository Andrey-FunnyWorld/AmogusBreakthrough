using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImposterSkinned : MonoBehaviour {
    public Animator Animator;
    public SkinnedMeshRenderer Renderer;
    public Transform HatPlaceholder;
    public void Laugh() {
        Animator.SetBool("laugh", true);
    }
    public void ApplyMaterials(Material colorMaterial) {
        Material[] updatedMaterials = Renderer.materials;
        updatedMaterials[0] = colorMaterial;
        Renderer.materials = updatedMaterials;
    }
    public void ApplyHat(Transform hat) {
        if (hat != null) {
            hat.SetParent(HatPlaceholder);
            hat.localPosition = Vector3.zero;
            hat.rotation = Quaternion.Euler(0, HatPlaceholder.parent.rotation.eulerAngles.y, 0);
        }
    }
}
