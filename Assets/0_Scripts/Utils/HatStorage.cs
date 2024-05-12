using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HatStorage : MonoBehaviour {
    public HatNamePrefabMap HatNamePrefabMap;
    public static HatStorage Instance;
    void Awake() {
        if (Instance == null) {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            Instance = this;
        } else {
            Destroy(gameObject);
        }
    }
    Dictionary<SkinItemName, List<HatStorageItem>> hats = new Dictionary<SkinItemName, List<HatStorageItem>>();
    public Transform GetHat(SkinItemName name) {
        if (!name.ToString().Contains("Hat")) Debug.LogError("HatStorage can only give hats");
        if (!hats.ContainsKey(name)) {
            hats.Add(name, new List<HatStorageItem>());
        }
        HatStorageItem item = hats[name].FirstOrDefault(h => !h.IsUsing);
        if (item == null) {
            Transform newHat = Instantiate(HatNamePrefabMap.Hats.First(h => h.SkinItemName == name).HatPrefab);
            hats[name].Add(new HatStorageItem() { Instance = newHat, IsUsing = false});
        }
        item = hats[name].FirstOrDefault(h => !h.IsUsing);
        item.Instance.gameObject.SetActive(true);
        item.IsUsing = true;
        return item.Instance;
    }
    public void RemoveHat(SkinItemName name, Transform instance) {
        HatStorageItem item = hats[name].First(h => h.Instance == instance);
        DetachItem(item);
    }
    public void RemoveHat(Transform instance) {
        SkinItemName name = hats.First(p => p.Value.Any(i => i.Instance == instance)).Key;
        RemoveHat(name, instance);
    }
    // call when loading levels
    public void DetachAllItems() {
        foreach (KeyValuePair<SkinItemName, List<HatStorageItem>> pair in hats) {
            foreach (HatStorageItem item in pair.Value) {
                DetachItem(item);
            }
        }
    }
    void DetachItem(HatStorageItem item) {
        item.IsUsing = false;
        item.Instance.transform.SetParent(transform);
        item.Instance.gameObject.SetActive(false);
    }
}

public class HatStorageItem {
    public Transform Instance;
    public bool IsUsing;
}
