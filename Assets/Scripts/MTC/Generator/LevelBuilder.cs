using System.Collections.Generic;
using UnityEngine;

public class LevelBuilder : MonoBehaviour
{
    [SerializeField]
    private int levelIndex = 0;

    [SerializeField]
    private int width = 10;
    
    [SerializeField] 
    private int height = 10;

    private List<Vehicle> cars;
    private List<Wall> walls;
    private List<Obstacle> obstacles;

    /// <summary>
    /// This function generates the parking lot ground
    /// based on the size and also places walls around lot
    /// </summary>
    public void GenerateWalls()
    {
        Debug.Log($"Generating walls for level {levelIndex}");
    }

    /// <summary>
    /// This function places random obstacles like
    /// trashcan or barrels etc. inside the parking lot
    /// </summary>
    public void GenerateObstacles()
    {
        Debug.Log($"Generating obstacles for level {levelIndex}");
    }
    
    /// <summary>
    /// After generating walls we can generate cars
    /// and place them in an order so that all of them
    /// can be moved out from the lot
    /// </summary>
    public void GenerateCars()
    {
        Debug.Log($"Generating cars for level {levelIndex}");
    }
    
    /// <summary>
    /// this function generates full level,
    /// calls all generator functions
    /// </summary>
    public void GenerateAll()
    {
        Debug.Log($"Generating level {levelIndex}");
    }
}
