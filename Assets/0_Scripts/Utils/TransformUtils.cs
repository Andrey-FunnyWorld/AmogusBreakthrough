using UnityEngine;

public static class TransformUtils
{
    public static void DeactivateChildren(this Transform transform) {
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }
}
