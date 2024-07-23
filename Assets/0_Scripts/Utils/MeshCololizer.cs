using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshCololizer : MonoBehaviour {
    public Color Color;
    public int MaterialIndex;
    public MeshRenderer MeshRenderer;
    Material newMaterial;
    void Start() {
        if (newMaterial == null) ApplyStartColor();
    }
    void ApplyStartColor() {
        newMaterial = new Material(MeshRenderer.materials[MaterialIndex]);
        newMaterial.color = Color;
        List<Material> materials = new List<Material>(MeshRenderer.materials);
        materials[MaterialIndex] = newMaterial;
        MeshRenderer.SetMaterials(materials);
    }
    public void ChangeColor(Color newColor) {
        if (newMaterial == null) ApplyStartColor();
        newMaterial.SetColor("_Color", newColor);
    }
}
