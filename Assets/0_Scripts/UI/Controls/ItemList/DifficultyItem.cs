using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DifficultyItem : MonoBehaviour, IPointerDownHandler {
    public Difficulty Difficulty;
    public void OnPointerDown(PointerEventData arg) {
        // run level with Difficulty
    }
}
public enum Difficulty {
    Noob, Pro, Hacker
}