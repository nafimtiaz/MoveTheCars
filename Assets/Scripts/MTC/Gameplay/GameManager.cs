using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private MTCGameConfig gameConfig;
    public MTCGameConfig Config => gameConfig;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }
        Instance = this;

        Application.targetFrameRate = gameConfig.targetFPS;
    }

    public static MTCGameConfig GetConfig()
    {
        return Instance.gameConfig;
    }
}
