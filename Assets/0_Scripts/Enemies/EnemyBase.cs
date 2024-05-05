using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : RoadObjectBase {
    public float MoveSpeed;
    protected abstract void Attack();
    public override void Destroyed() {
        // play death animation, give coins and destroy object
        Destroy(gameObject);
    }
}
