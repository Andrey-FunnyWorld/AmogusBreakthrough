using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class LevelUIManager : MonoBehaviour {
    public DefeatUI DefeatUI;
    public Transform TouchControls;
    void Update() {
        if (Input.GetKeyDown(KeyCode.B)) {
            DefeatViewModel vm = new DefeatViewModel() { CoinReward = 70 };
            DefeatUI.ShowResult(vm);
        }
    }
    public void AdjustToPlatform(PlatformType platformType) {
        IPlatformAdaptable[] adaptables = GetComponentsInChildren<IPlatformAdaptable>();
        foreach (IPlatformAdaptable adaptable in adaptables)
            adaptable.Adapt(platformType);
        //TouchControls.gameObject.SetActive(platformType != PlatformType.Desktop);
    }
}
public interface IPlatformAdaptable {
    void Adapt(PlatformType platformType);
}