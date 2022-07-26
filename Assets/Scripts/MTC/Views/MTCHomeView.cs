using System;
using System.Collections;
using System.Collections.Generic;
using MTC.Utils;
using UnityEngine;

public class MTCHomeView : MonoBehaviour
{
    public LevelData LevelData => currentLevelData;
    private LevelData currentLevelData;


    private void Start()
    {
        GenerateScene();
    }
    
    void GenerateScene()
    {
        PlaceParkingLotObjects(0);
    }

    void PlaceParkingLotObjects(int levelIndex)
    {
        GameData gameData = DataManager.LoadDataFromJson<GameData>();
        currentLevelData = gameData.levelData[levelIndex];

        foreach (var objData in currentLevelData.lotObjects)
        {
            ObjectCreator.CreateAndPlaceObject(objData);
        }

        ObjectCreator.CreateParkingLot(currentLevelData.length, currentLevelData.width);
    }
}
