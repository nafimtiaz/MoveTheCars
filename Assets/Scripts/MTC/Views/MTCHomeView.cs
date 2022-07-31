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

    private List<GameObject> currentPoolObjects;


    #region Create/Clear Scene
    
    private readonly Vector3[] roadRotations =
    {
        new Vector3(0f,0f,0f),
        new Vector3(0f,-90f,0f),
        new Vector3(0f,180f,0f),
        new Vector3(0f,90f,0f),
    };
    
    private Vector3[] translateUnit =
    {
        Vector3.right, 
        Vector3.forward, 
        Vector3.left, 
        Vector3.back
    };
    public void GenerateScene(int levelIndex)
    {
        ClearLevel();
        PlaceParkingLotObjects(levelIndex);
    }

    void PlaceRoads(int length, int width)
    {
        parkingLotGround.transform.localScale = new Vector3(length, 1f, width);
        roadCorners[0].localPosition = new Vector3(length, 0f, 0f);
        roadCorners[1].localPosition = new Vector3(length, 0f, width);
        roadCorners[2].localPosition = new Vector3(0f, 0f, width);
        
        bool isLength = false;
        Vector3 currentPos = Vector3.zero;
        
        for (int side = 0; side < 4; side++)
        {
            isLength = !isLength;
            int roadCount = isLength ? length : width;

            for (int n = 0; n < roadCount; n++)
            {
                if (side == 0 && n < 2)
                {
                    currentPos += translateUnit[side];
                }
                else
                {
                    GameObject roadObject = GameManager.GetPool().GetObjectFromPool("RoadDefault");
                    currentPoolObjects.Add(roadObject);
                    roadObject.SetActive(true);
                    roadObject.transform.SetParent(parkingLotParent);
                    roadObject.transform.localPosition = currentPos;
                    roadObject.transform.localEulerAngles = roadRotations[side];
                    currentPos += translateUnit[side];
                }
            }
        }
    }

    void PlaceParkingLotObjects(int levelIndex)
    {
        GameData gameData = DataManager.LoadDataFromJson<GameData>();
        currentLevelData = gameData.levelData[levelIndex];

        foreach (var objData in currentLevelData.lotObjects)
        {
            BaseParkingLotObject obj = ObjectCreator.CreateAndPlaceObject(objData,true);
            currentPoolObjects.Add(obj.gameObject);
            obj.gameObject.SetActive(true);
            obj.transform.SetParent(parkingLotParent);
        }

        PlaceRoads(currentLevelData.length, currentLevelData.width);
    }

    void ClearLevel()
    {
        if (currentPoolObjects != null)
        {
            for (int i = 0; i < currentPoolObjects.Count; i++)
            {
                GameManager.GetPool().ReturnObjectToPool(currentPoolObjects[i]);
            }   
        }

        currentPoolObjects = new List<GameObject>();
    }

    #endregion
}
