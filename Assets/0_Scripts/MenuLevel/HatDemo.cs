using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HatDemo : MonoBehaviour {
    public float Duration = 3;
    public float RotationSpeed = 40;
    Transform currentItem = null;
    void Start() {
        ChangeItem();
        StartCoroutine(Loop(Duration));
    }
    IEnumerator Loop(float time) {
        float timer = 0;
        while (timer < time) {
            timer += Time.deltaTime;
            yield return null;
        }
        ChangeItem();
        StartCoroutine(Loop(time));
    }
    void ChangeItem() {
        int index = Random.Range(0, HatStorage.Instance.HatNamePrefabMap.Hats.Length);
        Transform hatPrefab = HatStorage.Instance.HatNamePrefabMap.Hats[index].HatPrefab;
        if (currentItem != null)
            Destroy(currentItem.gameObject);
        currentItem = Instantiate(hatPrefab, transform);
        currentItem.GetChild(0).localPosition = Vector3.zero;
        currentItem.gameObject.SetActive(true);
        Spiner spiner = currentItem.gameObject.AddComponent<Spiner>();
        spiner.Speed = RotationSpeed;
        spiner.Axis = Axis.Y;
        currentItem.localPosition = Vector2.zero;
    }
}
