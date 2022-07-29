using System;
using System.Collections;
using System.Collections.Generic;
using MTC.Core.Classes;
using MTC.Utils;
using UnityEngine;

public class MTCHomeView : MonoBehaviour
{
    public LevelData LevelData => currentLevelData;
    private LevelData currentLevelData;
    [SerializeField] private Transform parkingLotParent;
    [SerializeField] private Transform parkingLotGround;
    [SerializeField] private Transform[] roadCorners;

    public void GenerateScene(int levelIndex)
    {
        PlaceParkingLotObjects(levelIndex);
    }

    void PlaceLotAndCorners(int length, int width)
    {
        parkingLotGround.transform.localScale = new Vector3(length, 1f, width);
        roadCorners[0].localPosition = new Vector3(length, 0f, 0f);
        roadCorners[1].localPosition = new Vector3(length, 0f, width);
        roadCorners[2].localPosition = new Vector3(0f, 0f, width);
    }

    void PlaceParkingLotObjects(int levelIndex)
    {
        GameData gameData = DataManager.LoadDataFromJson<GameData>();
        currentLevelData = gameData.levelData[levelIndex];

        foreach (var objData in currentLevelData.lotObjects)
        {
            BaseParkingLotObject obj = ObjectCreator.CreateAndPlaceObject(objData);
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(parkingLotParent);
        }

        PlaceLotAndCorners(currentLevelData.length, currentLevelData.width);
    }
}
