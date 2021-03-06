using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "MTC/MTCGameConfig", order = 1)]
public class MTCGameConfig : ScriptableObject
{
    [Header("Common")]
    public int targetFPS;
    public float vehicleSpeedUnitsPerSecond;
    public float vehicleLinearSpeed;
    public float vehicleTurningTime;
    
    [Header("Pooling")]
    public int vehiclePoolCount;
    public int wallPoolCount;
    public int obstaclePoolCount;
    public int roadPoolCount;
    
    public GameObject[] vehicles;
    public GameObject[] walls;
    public GameObject[] obstacles;
    public GameObject[] roads;

    [Header("Parking Lot")]
    public Vector3 lotColliderPosition;
    public Vector3 lotColliderScale;

    [Header("Vehicle")]
    public Vector3 vehicleColliderPosition;
    public Vector3 vehicleColliderScale;

    [Header("Obstacle")]
    public Vector3 obstacleColliderPosition;
    public Vector3 obstacleColliderScale;

    [Header("Wall")]
    public Vector3 wallColliderPosition;
    public Vector3 wallColliderScale;
    
    
    [Header("Environment")]
    public Vector3 roadColliderPosition;
    public Vector3 roadColliderScale;
}