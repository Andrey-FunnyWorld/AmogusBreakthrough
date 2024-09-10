using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyArmoredGiant : EnemyGiant {
    public Transform Shield;
    public float HpToHideShield = 25;
    protected override void HpChanged(float newValue) {
        base.HpChanged(newValue);
        if (Shield.gameObject.activeSelf && newValue < HpToHideShield) {
            Shield.gameObject.SetActive(false);
        }
    }
}
