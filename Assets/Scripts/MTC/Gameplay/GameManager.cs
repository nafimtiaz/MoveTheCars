using System;
using System.Collections;
using System.Collections.Generic;
using MTC.Gameplay;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private MTCGameConfig gameConfig;

    private PoolManager PoolManager;

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

    private void Start()
    {
        CreateObjectPool();
    }

    private void CreateObjectPool()
    {
        PoolManager = new PoolManager();
        PoolManager.CreatePool();
    }

    public static MTCGameConfig GetConfig()
    {
        return Instance.gameConfig;
    }
    
    public static PoolManager GetPool()
    {
        return Instance.PoolManager;
    }
}
