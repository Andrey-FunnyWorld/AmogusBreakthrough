using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DropPlatform : MonoBehaviour {
    public Animator Animator;
    public Transform ObjectToDrop;
    public bool DestroyObjectAfterDrop = true;
    public float FlyTime = 0.5f;
    public float FlyDistance = 3f;
    public Material ImposterChecked;
    public Renderer StatusPlane;
    public UnityEvent<int> DropAction;
    public int Index;
    bool canSelect = false;
    public void Drop() {
        Animator.SetBool("drop", true);
        StartCoroutine(DropObject(ObjectToDrop));
        DropAction.Invoke(Index);
    }
    public void AssignDropObject(Transform dropObject) {
        dropObject.SetParent(ObjectToDrop);
        dropObject.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(0, 90, 0));
    }
    IEnumerator DropObject(Transform objectToDrop) {
        float endPos = objectToDrop.position.y - FlyDistance;
        float startYPos = objectToDrop.position.y;
        float timer = 0;
        while (objectToDrop.position.y > endPos) {
            timer += Time.deltaTime;
            float distance = Utils.Gravity * (timer * timer) / 2;
            objectToDrop.position = new Vector3(objectToDrop.position.x, startYPos - distance, objectToDrop.position.z);
            yield return null;
        }
        if (DestroyObjectAfterDrop)
            Destroy(objectToDrop.gameObject);
    }
    public void SetChecked() {
        StatusPlane.material = ImposterChecked;
    }
    public void ReadyForSelection() {
        canSelect = true;
        Animator.SetBool("question", true);
    }
    public void BlockSelection() {
        canSelect = false;
    }
    void OnMouseDown() {
        HandleClick();
    }
    void OnTouchDown() {
        HandleClick();
    }
    void HandleClick() {
        if (canSelect) {
            Drop();
            Animator.SetBool("question", false);
        }
    }
}
