using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshCololizer : MonoBehaviour {
    public Color Color;
    public int MaterialIndex;
    public MeshRenderer MeshRenderer;
    void Start() {
        Material newMat = new Material(MeshRenderer.materials[MaterialIndex]);
        newMat.color = Color;
        List<Material> materials = new List<Material>(MeshRenderer.materials);
        materials[MaterialIndex] = newMat;
        MeshRenderer.SetMaterials(materials);
    }
}
