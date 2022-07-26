using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MTC.Core.Classes;
using MTC.Core.Enums;
using MTC.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField]
    private bool shouldDrawGizmos = false;
    
    [SerializeField]
    private int levelIndex = 0;

    [SerializeField]
    private int width = 10;
    
    [SerializeField] 
    private int length = 10;
    
    [SerializeField] 
    private int numOfObstacles = 3;
    
    [SerializeField] 
    private int minLinGapObstacles = 2;
    
    [SerializeField] [TextArea] 
    private string levelMessage;
    
    // Wall vars
    private List<BaseParkingLotObject> walls;
    private List<List<bool>> wallMask;
    private List<Dictionary<Vector3,int>> wallGapPositions;

    private List<BaseParkingLotObject> vehicles;
    private List<BaseParkingLotObject> obstacles;
    private List<BaseParkingLotObject> allParkingLotObjects;
    private GameData gameData;
    private bool isBusy;

    private Vector3[] translateUnit =
    {
        Vector3.right, 
        Vector3.forward, 
        Vector3.left, 
        Vector3.back
    };
    
    private Vector3[] globalDirs =
    {
        Vector3.forward, 
        Vector3.left, 
        Vector3.back, 
        Vector3.right
    };

    #region Level Generator UI Callbacks

    /// <summary>
    /// This function generates the parking lot ground
    /// based on the size and also places walls around lot
    /// </summary>
    public void GenerateWalls()
    {
        if (!isBusy)
        {
            isBusy = true;
            ClearAll();
            GenerateWallsAroundLot(() => isBusy = false);   
        }
    }

    /// <summary>
    /// This function places random obstacles like
    /// trashcan or barrels etc. inside the parking lot
    /// </summary>
    public void GenerateObstacles()
    {
        if (!isBusy && walls.Count > 0)
        {
            isBusy = true;
            ClearAllParkingLotObjectsByType(ParkingLotObjectType.Obstacle);
            ClearAllParkingLotObjectsByType(ParkingLotObjectType.Car);

            StartCoroutine(RefreshPossibleCarPositions(() =>
            {
                PlaceObstacles(() => isBusy = false);
            }));
        }
    }
    
    /// <summary>
    /// After generating walls we can generate cars
    /// and place them in an order so that all of them
    /// can be moved out from the lot
    /// </summary>
    public void GenerateVehicles()
    {
        if (!isBusy && walls.Count > 0)
        {
            isBusy = true;
            ClearAllParkingLotObjectsByType(ParkingLotObjectType.Car);
            
            StartCoroutine(RefreshPossibleCarPositions(() =>
            {
                StartCoroutine(PlaceVehicles(() => isBusy = false));
            }));
        }
    }
    
    /// <summary>
    /// this function generates full level,
    /// calls all generator functions
    /// </summary>
    public void GenerateAll()
    {
        Debug.Log($"Generating level {levelIndex}");
        
        if (!isBusy)
        {
            isBusy = true;
            ClearAll();
            GenerateWallsAroundLot(() =>
            {
                StartCoroutine(RefreshPossibleCarPositions(() =>
                {
                    PlaceObstacles(() =>
                    {
                        StartCoroutine(RefreshPossibleCarPositions(() =>
                        {
                            StartCoroutine(PlaceVehicles(() => isBusy = false));
                        }));
                    });
                }));
            });   
        }
    }

    #endregion

    #region Wall Generation

    private void GenerateWallsAroundLot(Action OnComplete = null)
    {
        wallMask = GenerateWallsMask();
        wallGapPositions = new List<Dictionary<Vector3, int>>
        {
            new Dictionary<Vector3, int>(),
            new Dictionary<Vector3, int>(),
            new Dictionary<Vector3, int>(),
            new Dictionary<Vector3, int>()
        };
        
        Vector3 currentPos = Vector3.zero;

        for (int i = 0; i < 4; i++)
        {
            Vector3 currentRot = new Vector3(0, -90f * i, 0);
            
            for (int n = 0; n < wallMask[i].Count; n++)
            {
                if (wallMask[i][n])
                {
                    ParkingLotObjectData data = new ParkingLotObjectData(
                        ParkingLotObjectType.Wall,
                        "ConcreteWall",
                        currentPos,
                        currentRot);

                    var wall = ObjectCreator.CreateAndPlaceObject(data);
                    walls.Add(wall);
                    AddToParkingLotObjects(wall);
                }
                else
                {
                    wallGapPositions[i].Add((currentPos + translateUnit[i] * 0.5f - 
                                            globalDirs[i] * 0.125f + 
                                            new Vector3(0f,0.5f,0f)),0);
                }
                
                currentPos += translateUnit[i];
            }
        }
        
        StartCoroutine(RefreshPossibleCarPositions(() =>
        {
            if (OnComplete != null)
            {
                OnComplete();
            }
        }));
    }

    private List<int> GetWallSets() 
    {
        List<int> wallSet = new List<int>();
        float al = (width + length) / 2f;

        while (al/2 >= 1)
        {
            al /= 2f;
            al = Mathf.Ceil(al);
            wallSet.Add((int)al);
        }

        return wallSet;
    }
    
    // <WallCountInSet,NumberOfSets>
    private Dictionary<int,int> GetWallSetsWithCount()
    {
        Dictionary<int,int> wallSetCounts = new Dictionary<int,int>();
        List<int> wallSet = GetWallSets();
        int hw = (width + length);
        int nw;
        int rw = hw;

        nw = Mathf.FloorToInt((hw / 2f) / wallSet[0]);
        wallSetCounts.Add(wallSet[0], nw);
        rw = hw - (nw * wallSet[0]);

        for (int i = 1; i < wallSet.Count; i++)
        {
            nw = Mathf.FloorToInt(rw / (float)wallSet[i]);
            wallSetCounts.Add(wallSet[i], nw);
            rw -= (nw * wallSet[i]);

            if (rw == 0)
            {
                break;
            }
        }
        
        return wallSetCounts;
    }

    private List<int> GetShuffledWallSetSequence()
    {
        List<int> sequence = new List<int>();
        Dictionary<int, int> wallSets = GetWallSetsWithCount();

        foreach (var key in wallSets.Keys)
        {
            for (int i = 0; i < wallSets[key]; i++)
            {
                sequence.Add(key);
            }   
        }

        return sequence.GetShuffledList();
    }

    private List<List<bool>> GenerateWallsMask()
    {
        Dictionary<int, int> wallSetCount = GetWallSetsWithCount();
        
        List<int> ww = GetShuffledWallSetSequence();
        List<int> nw = GetShuffledWallSetSequence();
        
        Stack<bool> lnMask = new Stack<bool>();
        bool isWall = false;

        int iterCount = wallSetCount.Sum(x => x.Value);

        for (int i = 0; i < iterCount; i++)
        {
            for (int nwi = 0; nwi < nw[i]; nwi++)
            {
                lnMask.Push(isWall);
            }

            isWall = true;
            
            for (int wwi = 0; wwi < ww[i]; wwi++)
            {
                lnMask.Push(isWall);
            }
            
            isWall = false;
        }
        
        List<List<bool>> mask = new List<List<bool>>()
        {
            new List<bool>(),
            new List<bool>(),
            new List<bool>(),
            new List<bool>()
        };

        bool isLength = false;
        
        for (int si = 0; si < 4; si++)
        {
            isLength = !isLength;
            int wCount = isLength ? length : width;

            for (int n = 0; n < wCount; n++)
            {
                mask[si].Add(lnMask.Pop());
            }
        }

        return mask;
    }

    #endregion

    #region Obstacle Placement

    private void PlaceObstacles(Action OnComplete = null)
    {
        bool[] obsMask = GetObstacleMask();
        
        for (int l = 0; l < length; l++)
        {
            for (int w = 0; w < width; w++)
            {
                int posIndex = l * length + w;

                if (obsMask[posIndex])
                {
                    ParkingLotObjectData data = new ParkingLotObjectData(
                        ParkingLotObjectType.Obstacle,
                        "Barrel",
                        new Vector3(l, 0f, w),
                        Vector3.zero);

                    var obsatacle = ObjectCreator.CreateAndPlaceObject(data);
                    obstacles.Add(obsatacle);
                    AddToParkingLotObjects(obsatacle);
                }
            }
        }

        if (OnComplete != null)
        {
            OnComplete();
        }
    }

    private bool[] GetObstacleMask()
    {
        bool[] obsMask = new bool[width * length];

        int maxPossibleObs = obsMask.Length / (minLinGapObstacles + 1);

        if (numOfObstacles > maxPossibleObs)
        {
            numOfObstacles = maxPossibleObs;
        }

        int linGap = obsMask.Length / (numOfObstacles + 1);

        int offsetUnits = linGap - minLinGapObstacles;

        for (int i = 0; i < numOfObstacles; i++)
        {
            obsMask[linGap * (i + 1) + Random.Range(minLinGapObstacles, offsetUnits)] = true;
        }

        return obsMask;
    }

    #endregion
    
    #region Vehicle Placement
    
    private IEnumerator RefreshPossibleCarPositions(Action OnComplete = null)
    {
        bool isAlongLength = true;

        for (int side = 0; side < wallGapPositions.Count; side++)
        {
            for (int i = 0; i < wallGapPositions[side].Count; i++)
            {
                var entry = wallGapPositions[side].ElementAt(i);

                if (Physics.Raycast(entry.Key,globalDirs[side],out var hit,(length + width)))
                {
                    wallGapPositions[side][entry.Key] = Mathf.FloorToInt(Vector3.Distance(entry.Key, hit.point));
                }
                else
                {
                    wallGapPositions[side][entry.Key] = isAlongLength? width : length;
                }

                yield return new WaitForFixedUpdate();
            }
            
            isAlongLength = !isAlongLength;
        }

        if (OnComplete != null)
        {
            OnComplete();
        }
    }

    private bool IsBlankSpaceAvailable(List<Dictionary<Vector3,int>> collection)
    {
        int entryCount = 0;

        foreach (var dict in collection)
        {
            entryCount += dict.Count(x => x.Value >= 2);
        }

        return entryCount > 0;
    }


    private string GetVehicleType()
    {
        var vehicleTypes = Enum.GetValues(typeof(VehicleType)) as VehicleType[];

        if (vehicleTypes != null)
        {
            return vehicleTypes[Random.Range(0, vehicleTypes.Length)].ToString();
        }

        return null;
    }

    private IEnumerator PlaceVehicles(Action OnComplete = null)
    {
        while (IsBlankSpaceAvailable(wallGapPositions))
        {
            for (int side = 0; side < 4; side++)
            {
                List<Vector3> checkPos = wallGapPositions[side].Keys.ToList();
                checkPos = checkPos.GetShuffledList();

                for (int i = 0; i < checkPos.Count; i++)
                {
                    int blankUnits = wallGapPositions[side][checkPos[i]];

                    if (blankUnits >= 2)
                    {
                        float placementUnit = Random.Range(1, blankUnits);
                        Vector3 pos = checkPos[i] + globalDirs[side] * placementUnit;
                        pos = new Vector3(pos.x, 0f, pos.z) + globalDirs[side] * 0.125f;
                        Vector3 addRotation = Random.Range(1, 100) <= 50 ? Vector3.zero : new Vector3(0f, 180f, 0f);
                        Vector3 rot = new Vector3(0f, -90f * side, 0f) + addRotation;
                        
                        ParkingLotObjectData data = new ParkingLotObjectData(
                            ParkingLotObjectType.Car,
                            GetVehicleType(),
                            pos,
                            rot);

                        var car = ObjectCreator.CreateAndPlaceObject(data);
                        car.gameObject.name = $"car side:{side}";
                        vehicles.Add(car);
                        AddToParkingLotObjects(car);
                        StartCoroutine(RefreshPossibleCarPositions());
                        yield return new WaitForSeconds(0.1f);
                        break;
                    }
                }
            }
        }
        
        if (OnComplete != null)
        {
            OnComplete();
        }
    }

    #endregion

    #region Saving Level

    public void SaveLevel()
    {
        #if UNITY_EDITOR
        
        LevelData currentLevelData = new LevelData(levelIndex, length, width, levelMessage, GetCurrentParkingLotObjectsData());
        GameData gameData = DataManager.LoadDataFromJson<GameData>();
        if (gameData.levelData != null)
        {
            gameData.levelData.RemoveAll(lvl => lvl.levelIndex == levelIndex);
        }
        else
        {
            gameData.levelData = new List<LevelData>();
        }
        gameData.levelData.Add(currentLevelData);
        gameData.levelData = gameData.levelData.OrderBy(lvl => lvl.levelIndex).ToList();
        DataManager.SaveDataToJson(gameData);
        
        #endif
    }

    private List<ParkingLotObjectData> GetCurrentParkingLotObjectsData()
    {
        List<ParkingLotObjectData> objData = new List<ParkingLotObjectData>();

        foreach (var obj in allParkingLotObjects)
        {
            objData.Add(new ParkingLotObjectData(
                obj.LotObjectType,
                obj.LotObjectSubType,
                obj.Position,
                obj.Rotation));
        }

        return objData;
    }

    #endregion
    
    #region Clearing

    public void ClearAll()
    {
        isBusy = true;
        StopAllCoroutines();

        ClearAllParkingLotObjectsByType(ParkingLotObjectType.Wall);
        ClearAllParkingLotObjectsByType(ParkingLotObjectType.Obstacle);
        ClearAllParkingLotObjectsByType(ParkingLotObjectType.Car);
        
        isBusy = false;
    }

    private void ClearAllParkingLotObjectsByType(ParkingLotObjectType type)
    {
        if (allParkingLotObjects != null)
        {
            foreach (var obj in allParkingLotObjects)
            {
                if (obj.LotObjectType == type)
                {
                    DestroyImmediate(obj.gameObject);
                }
            }
            
            allParkingLotObjects.RemoveAll(x => x.LotObjectType == type);
            
            allParkingLotObjects.TrimExcess();
        }

        switch (type)
        {
            case ParkingLotObjectType.Car:
                vehicles = new List<BaseParkingLotObject>();
                break;
            case ParkingLotObjectType.Obstacle:
                obstacles = new List<BaseParkingLotObject>();
                break;
            case ParkingLotObjectType.Wall:
                walls = new List<BaseParkingLotObject>();
                wallGapPositions = new List<Dictionary<Vector3, int>>();
                break;
        }
    }

    private void AddToParkingLotObjects(BaseParkingLotObject obj)
    {
        if (allParkingLotObjects == null)
        {
            allParkingLotObjects = new List<BaseParkingLotObject>();
        }
        
        allParkingLotObjects.Add(obj);
    }

    #endregion

    #region Misc

    private void OnDrawGizmos()
    {
        if (shouldDrawGizmos)
        {
            if (wallGapPositions != null)
            {
                for (int side = 0; side < wallGapPositions.Count; side++)
                {
                    for (int i = 0; i < wallGapPositions[side].Count; i++)
                    {
                        var entry = wallGapPositions[side].ElementAt(i);
                        Gizmos.color = Color.green;
                        Gizmos.DrawLine(entry.Key, entry.Key + globalDirs[side] * entry.Value);
                    }
                }
            }
        }
    }

    #endregion
}
