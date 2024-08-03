using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUIManager : MonoBehaviour {
    public DefeatUI DefeatUI;
    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            DefeatViewModel vm = new DefeatViewModel() { CoinReward = 70 };
            DefeatUI.ShowResult(vm);
        }
    }
}
