using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StartupViewModel {
    public string Locale;
    public PlatformType Platform;
    public bool IsLogged;
    public PlayerSettings Settings;
    public ProgressState Progress;
}

public enum PlatformType {
    Desktop, Android, IOS
}