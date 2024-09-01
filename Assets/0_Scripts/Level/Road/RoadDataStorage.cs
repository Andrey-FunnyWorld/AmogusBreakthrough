using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RoadDataStorage", menuName = "ScriptableObjects/RoadDataStorage", order = 2)]
public class RoadDataStorage : ScriptableObject {
    public RoadDataViewModel[] Levels;
}
