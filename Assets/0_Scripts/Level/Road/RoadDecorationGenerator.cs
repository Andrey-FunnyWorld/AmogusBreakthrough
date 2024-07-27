using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadDecorationGenerator : MonoBehaviour {
    public RoadDecoration[] FloorDecorationPrefabs;
    public RoadDecoration[] WallDecorationPrefabs;
    public float PositionDistanceToGenerateDecor = 20;
    public float LeftX, RightX;
    public float MaxPositionToNextDecor = 5;
    public float MinPositionToNextDecor = 1;
    public float SideRangeX;
    public FloatRange PositionException;
    float positionToGenerate = 0;
    public RoadObjectBase PositionChanged(float newPosition) {
        if (PositionDistanceToGenerateDecor + newPosition >= positionToGenerate) {
            RoadObjectBase decor = GenerateDecoration(PositionDistanceToGenerateDecor + newPosition);
            positionToGenerate += Random.Range(MinPositionToNextDecor, MaxPositionToNextDecor);
            return decor;
        }
        return null;
    }
    RoadObjectBase GenerateDecoration(float roadPosition) {
        bool isWall = Random.Range(0, 4) == 0;
        return GenerateDecorationBase(roadPosition, !isWall);
    }
    RoadObjectBase GenerateDecorationBase(float roadPosition, bool isFloor) {
        RoadDecoration[] prefabs = isFloor ?  FloorDecorationPrefabs : WallDecorationPrefabs;
        int index = Random.Range(0, prefabs.Length);
        RoadDecoration newDecoration = Instantiate(prefabs[index]);
        newDecoration.RoadPosition = roadPosition;
        if (isFloor)
            newDecoration.transform.position = new Vector3(CalcDecorX(), 0, 0);
        if (newDecoration.RandomRotate)
            newDecoration.transform.Rotate(0, Random.Range(0, 360), 0);
        return newDecoration;
    }
    float CalcDecorX() {
        bool isLeft = Random.Range(0, 2) == 0;
        float x = Random.Range(0, SideRangeX);
        return isLeft ? LeftX + x : RightX - x;
    }
    public List<RoadObjectBase> GenerateStartDecoration() {
        List<RoadObjectBase> decorations = new List<RoadObjectBase>();
        positionToGenerate = Random.Range(MinPositionToNextDecor, MaxPositionToNextDecor);
        while (positionToGenerate < PositionDistanceToGenerateDecor) {
            RoadObjectBase decor = GenerateDecoration(positionToGenerate);
            decorations.Add(decor);
            positionToGenerate += Random.Range(MinPositionToNextDecor, MaxPositionToNextDecor);
            while (PositionException.Include(positionToGenerate)) {
                positionToGenerate += Random.Range(MinPositionToNextDecor, MaxPositionToNextDecor);
            }
        }
        return decorations;
    }
}
[System.Serializable]
public class FloatRange {
    public float Start, End;
    public bool Include(float pos) {
        return pos > Start && pos < End;
    }
}