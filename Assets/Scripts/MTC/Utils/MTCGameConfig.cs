using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "MTC/MTCGameConfig", order = 1)]
public class MTCGameConfig : ScriptableObject
{
    
    /// <summary>
    /// This striptable object is our CMS
    /// </summary>
    
    [Header("Common")]
    public int targetFPS;
    public float vehicleSpeedUnitsPerSecond;
    public float vehicleLinearSpeed;
    public float vehicleTurningTime;
    public LayerMask vehicleLayerMask;
    
    public GameObject impactParticle;
    public GameObject explosionParticle;
    public GameObject celebrationParticle;
    public GameObject camMinSize;
    public GameObject camMaxSize;
    
    [Header("Sounds")]
    public AudioClip menuMusicLoop;
    public AudioClip gameMusicLoop;
    public AudioClip impactSound;
    public AudioClip[] fireworkSounds;
    public AudioClip celebrationSound;
    public AudioClip scoreSound;
    public AudioClip explosionSound;
    public AudioClip carSound1;
    public AudioClip carSound2;
    
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