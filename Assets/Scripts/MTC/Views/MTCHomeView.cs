using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MTC.Core.Classes;
using MTC.Core.Enums;
using MTC.Gameplay;
using MTC.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MTCHomeView : MonoBehaviour
{
    [Header("Common")] 
    [SerializeField] private Camera mainCam;
    [SerializeField] private TouchManager touchManager;
    
    [Header("UI Elemenets")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject levelSelectionPanel;
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject levelMessagePanel;
    [SerializeField] private GameObject confirmationPanel;
    [SerializeField] private GameObject inGameUIPanel;
    
    
    [SerializeField] private Button menuPlayButton;
    [SerializeField] private Button menuQuitButton;
    [SerializeField] private Button[] levelSelectionButtons;
    [SerializeField] private Button levelSelectionBackButton;
    [SerializeField] private TextMeshProUGUI levelCompletionMessageText;
    [SerializeField] private Button[] menuButtons;
    [SerializeField] private Button[] restartButtons;
    [SerializeField] private TextMeshProUGUI levelMessageText;
    [SerializeField] private Button closeMessageButton;
    [SerializeField] private Button confirmationCloseButton;
    [SerializeField] private TextMeshProUGUI confirmationMessageText;
    [SerializeField] private Button confirmationYesButton;
    [SerializeField] private TextMeshProUGUI levelName;
    [SerializeField] private GameObject confettiGroup;
    [SerializeField] private EmoView[] emoBubbles;


    [Header("Parking Lot Generation")]
    [SerializeField] private Transform parkingLotParent;
    [SerializeField] private Transform parkingLotGround;
    [SerializeField] private Transform[] roadCorners;
    public LevelData LevelData => currentLevelData;
    private LevelData currentLevelData;
    private List<GameObject> currentPoolObjects;
    private int vehicleCount;

    private bool isLevelComplete => vehicleCount >=
                                    currentLevelData.lotObjects
                                        .Where(x => x.lotObjectType == ParkingLotObjectType.Car)
                                        .ToArray().Length;

    void Start()
    {
        touchManager.ToggleTouch(false);
        PopulateCallbacks();
        GameManager.GetSoundManager().SetBackgroundMusic(GameManager.GetConfig().menuMusicLoop);
    }

    private void SetCameraStatus(LevelData currentLevelData)
    {
        Vector2 camVector = new Vector2(currentLevelData.length, currentLevelData.width);
        mainCam.transform.LookAt(new Vector3(currentLevelData.length / 2f, 0f, currentLevelData.width / 2f));
        mainCam.orthographicSize = Mathf.Lerp(7f, 11f, (camVector.magnitude - 4f) / 9f);
    }

    #region UI Callbacks

    private void PopulateCallbacks()
    {
        menuPlayButton.onClick.AddListener(OnMainMenuPlayButtonClicked);
        menuQuitButton.onClick.AddListener(() => Application.Quit());
        levelSelectionBackButton.onClick.AddListener(OnClickLevelSelectionbackButton);
        confirmationCloseButton.onClick.AddListener(() => confirmationPanel.SetActive(false));

        foreach (var btn in menuButtons)
        {
            btn.onClick.AddListener(OnClickMenuButton);
        }
        
        foreach (var btn in restartButtons)
        {
            btn.onClick.AddListener(OnClickRestartButton);
        }
    }

    private void OnClickMenuButton()
    {
        if (isLevelComplete)
        {
            OpenMenu();
        }
        else
        {
            confirmationMessageText.text = "Are you sure you want to quit to menu?";
            confirmationPanel.SetActive(true);
            confirmationYesButton.onClick.RemoveAllListeners();
            confirmationYesButton.onClick.AddListener(OpenMenu);
        }
    }
    
    private void OnClickRestartButton()
    {
        if (isLevelComplete)
        {
            RestartLevel();
        }
        else
        {
            confirmationMessageText.text = "Are you sure you want to restart?";
            confirmationPanel.SetActive(true);
            confirmationYesButton.onClick.RemoveAllListeners();
            confirmationYesButton.onClick.AddListener(RestartLevel);
        }
    }

    private void OpenMenu()
    {
        confettiGroup.SetActive(false);
        confirmationPanel.SetActive(false);
        levelCompletePanel.SetActive(false);
        inGameUIPanel.SetActive(false);
        levelSelectionPanel.SetActive(true);
        OnSwitchToMenu();
    }

    private void RestartLevel()
    {
        confettiGroup.SetActive(false);
        levelCompletePanel.SetActive(false);
        confirmationPanel.SetActive(false);
        GenerateScene(currentLevelData.levelIndex, ()=>
        {
            touchManager.ToggleTouch(true);
            inGameUIPanel.SetActive(true);
        });
    }

    private void OnClickLevelSelectionbackButton()
    {
        mainMenuPanel.SetActive(true);
        levelSelectionPanel.SetActive(false);
    }

        private void OnMainMenuPlayButtonClicked()
    {
        mainMenuPanel.SetActive(false);
        levelSelectionPanel.SetActive(true);
        
        // We set to 8 levels for now
        // this is a temporary solution
        // normally we will populate the view using a separate view class
        for (int i = 0; i < 8; i++)
        {
            TextMeshProUGUI btnText = levelSelectionButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            btnText.text = (i + 1).ToString();
            int lvlIndex = i + 1;
            levelSelectionButtons[i].onClick.AddListener((() => OnLevelButtonClicked(lvlIndex)));
        }
    }

    private void OnLevelButtonClicked(int levelIndex)
    {
        GenerateScene(levelIndex, () =>
        {
            levelSelectionPanel.SetActive(false);
            ShowLevelMessagePopup(currentLevelData.levelMessage);
        });
    }

    private void ShowLevelMessagePopup(string levelMsg)
    {
        if (!string.IsNullOrEmpty(levelMsg))
        {
            levelMessageText.text = levelMsg;
            levelMessagePanel.SetActive(true);
            touchManager.ToggleTouch(false);
            closeMessageButton.onClick.AddListener(()=>
            {
                touchManager.ToggleTouch(true);
                inGameUIPanel.SetActive(true);
                levelMessagePanel.SetActive(false);
            });
        }
        else
        {
            inGameUIPanel.SetActive(true);
            touchManager.ToggleTouch(true);
        }
    }

    #endregion
    
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
    public void GenerateScene(int levelIndex, Action OnComplete = null)
    {
        ClearLevel();
        vehicleCount = 0;
        OnGameStart();
        PlaceParkingLotObjects(levelIndex);
        SetCameraStatus(currentLevelData);
        levelName.text = $"LEVEL {levelIndex}";

        if (OnComplete != null)
        {
            OnComplete();
        }
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
        currentLevelData = gameData.levelData.Find(x => x.levelIndex == levelIndex);

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

    #region Game Flow

    public void AddVehicleCount()
    {
        vehicleCount++;

        if (isLevelComplete)
        {
            OnLevelWin();
        }
    }

    public void OnLevelWin()
    {
        levelCompletionMessageText.text = $"Congrats! You completed level {currentLevelData.levelIndex}!";
        confettiGroup.SetActive(true);
        inGameUIPanel.SetActive(false);
        levelCompletePanel.SetActive(true);
        GameManager.GetSoundManager().PlaySound(GameManager.GetConfig().celebrationSound);
    }
    
    public void OnGameOver()
    {
        levelCompletionMessageText.text = "Game Over!";
        inGameUIPanel.SetActive(false);
        levelCompletePanel.SetActive(true);
        touchManager.ToggleTouch(false);
    }

    public void AssignEmoBubble(Transform targetVehicle, bool isPositive)
    {
        for (int i = 0; i < emoBubbles.Length; i++)
        {
            if (!emoBubbles[i].gameObject.activeInHierarchy)
            {
                emoBubbles[i].TriggerEmo(targetVehicle,isPositive);
                break;
            }
        }
    }

    private void OnGameStart()
    {
        GameManager.GetSoundManager().SetBackgroundMusic(GameManager.GetConfig().gameMusicLoop);
    }

    private void OnSwitchToMenu()
    {
        GameManager.GetSoundManager().SetBackgroundMusic(GameManager.GetConfig().menuMusicLoop);
    }

    #endregion
}
