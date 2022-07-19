using System.Collections.Generic;
using System.Linq;
using MTC.Core.Classes;
using MTC.Core.Enums;
using MTC.Utils;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField]
    private int levelIndex = 0;

    [SerializeField]
    private int width = 10;
    
    [SerializeField] 
    private int length = 10;
    
    // Wall vars
    private List<BaseParkingLotObject> walls;
    private List<List<bool>> wallMask;

    private List<BaseParkingLotObject> cars;
    private List<BaseParkingLotObject> obstacles;
    private List<GameObject> allParkingLotObjects;
    private GameData gameData;

    private Vector3[] translateUnit =
    {
        new Vector3(1,0,0),
        new Vector3(0,0,1),
        new Vector3(-1,0,0),
        new Vector3(0,0,-1)
    };

    #region Level Generator UI Callbacks

    /// <summary>
    /// This function generates the parking lot ground
    /// based on the size and also places walls around lot
    /// </summary>
    public void GenerateWalls()
    {
        ClearAll();
        GenerateWallsAroundLot();
    }

    /// <summary>
    /// This function places random obstacles like
    /// trashcan or barrels etc. inside the parking lot
    /// </summary>
    public void GenerateObstacles()
    {

    }
    
    /// <summary>
    /// After generating walls we can generate cars
    /// and place them in an order so that all of them
    /// can be moved out from the lot
    /// </summary>
    public void GenerateCars()
    {

    }
    
    /// <summary>
    /// this function generates full level,
    /// calls all generator functions
    /// </summary>
    public void GenerateAll()
    {
        Debug.Log($"Generating level {levelIndex}");
    }

    #endregion

    #region Wall Generation

    private void GenerateWallsAroundLot()
    {
        wallMask = GenerateWallsMask();

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
                
                currentPos += translateUnit[i];
            }
        }
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
    
    #region Clearing

    public void ClearAll()
    {
        if (allParkingLotObjects != null)
        {
            foreach (var obj in allParkingLotObjects)
            {
                DestroyImmediate(obj);
            }
            
            allParkingLotObjects.Clear();
            allParkingLotObjects.TrimExcess();
        }

        walls = new List<BaseParkingLotObject>();
        cars = new List<BaseParkingLotObject>();
        obstacles = new List<BaseParkingLotObject>();
    }

    private void AddToParkingLotObjects(BaseParkingLotObject obj)
    {
        if (allParkingLotObjects == null)
        {
            allParkingLotObjects = new List<GameObject>();
        }
        
        allParkingLotObjects.Add(obj.gameObject);
    }

    #endregion
}
