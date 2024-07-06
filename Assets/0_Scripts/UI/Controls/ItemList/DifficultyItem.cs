using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DifficultyItem : MonoBehaviour, IPointerDownHandler {
    public Difficulty Difficulty;
    public LevelLoader LevelLoader;
    bool canPress = true;
    public void OnPointerDown(PointerEventData arg) {
        if (canPress) {
            canPress = false;
            LevelLoader.Difficulty = Difficulty;
            LevelLoader.LoadScene(LevelLoader.BATTLE_BUILD_INDEX);
        }
    }
}
public enum Difficulty {
    Noob, Pro, Hacker
}