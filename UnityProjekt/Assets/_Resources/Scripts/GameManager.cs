﻿using System.Linq;
using UnityEditor;
using UnityEngine;

public enum PlayerClasses
{
    BORO,
    JACK,
    COUNT
}

public enum PlayerSlots
{
    ONE,
    TWO,
    THREE,
    FOUR,
    COUNT
}

public class GameManager : MonoBehaviour {

    #region Singleton

    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if(!instance)
                instance = FindObjectOfType<GameManager>();

            return instance;
        }
    }

    void Awake()
    {
        instance = this;

        GameEventHandler.OnPause += OnPause;
        GameEventHandler.OnResume += OnResume;
    }

    #endregion

    public int CurrentSpawnedEntityCount = 0;
    public int MaxSpawnedEntityCount = 40;

    public static bool CanSpawnEntity
    {
        get { return (instance.CurrentSpawnedEntityCount < instance.MaxSpawnedEntityCount); }
    }

    public void AddEntity()
    {
        CurrentSpawnedEntityCount++;
    }

    public void RemoveEntity()
    {
        CurrentSpawnedEntityCount--;
    }

    public int currentPlayerSelectingClass = 0;

    public UIButton[] slotButtons;

    public Transform SpawnPosition;

    private PlayerController[] Players = {null,null,null,null};
    private CameraController[] Cameras = {null,null,null,null};

    public CameraController[] GetCameras()
    {
        return Cameras;
    }

    public static float CurrentDifficulty = 0;
    public static float DifficultyValue = 0;

    void Start()
    {
        GameEventHandler.TriggerOnPause();
    }

    public void RemovePlayer(PlayerController player)
    {
        if (player == null)
            return;

        int index = 0;
        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == player)
            {
                index = i;
                break;
            }
        }

        Destroy(Cameras[index].gameObject);
        Cameras[index] = null;

        GameEventHandler.TriggerPlayerLeft(player, player.Name);

        Players[index] = null;
        Destroy(player.gameObject);
    }

    public PlayerController MainPlayer
    {
        get
        {
            for (int i = 0; i < Players.Length; i++)
            {
                if (Players[i] != null)
                {
                    return Players[i];
                }
            }
            return null;
        }
    }

    public UIRect DifficultyBar;
    public UIText DifficultyText;

    void Update()
    {

        if (currentPlayerSelectingClass != -1 && slotButtons.Length > 0)
        {
            slotButtons[currentPlayerSelectingClass].Text = "Player " + (currentPlayerSelectingClass + 1) + " Select";
            slotButtons[currentPlayerSelectingClass].ButtonStyle.normal.textColor = Color.red;
        }



        if (GamePaused)
            return;
        DifficultyValue += Time.deltaTime;
        DifficultyBar.RelativeSize.y = DifficultyValue / 10.0f;
        if (DifficultyValue >= 5)
        {
            CurrentDifficulty++;
            DifficultyValue = 0;
            DifficultyText.Text = "Difficulty: " + CurrentDifficulty.ToString("####0");
        }
    }

    public int PlayerCount
    {
        get { return Players.Count(o => o != null); }
    }

    public void StartGame()
    {

        DifficultyValue = 0;
        CurrentDifficulty = 0;
        DifficultyText.Text = "Difficulty: " + CurrentDifficulty.ToString("####0");

        foreach (var playerController in Players)
        {
            if (playerController)
            {
                playerController.gameObject.SetActive(true);
                playerController.OnReset();
            }
        }

        int camIndex = 0;

        foreach (var cameraController in Cameras)
        {
            if (cameraController)
            {
                if (PlayerCount == 2)
                {
                    if (camIndex == 0)
                    {
                        cameraController.camera.rect = new Rect(0, 0, 1f, 0.5f);
                        
                    }
                    if (camIndex == 1)
                    {
                        cameraController.camera.rect = new Rect(0, 0.5f, 1f, 0.5f);
                        cameraController.GetComponent<PlayerUI>().panel.RelativePosition.y = 0.5f;
                    }
                }

                cameraController.gameObject.SetActive(true);

                camIndex++;
            }
        }

        GameContainer.SetActive(true);

        GameEventHandler.TriggerOnResume();
    }

    public void SelectSlot(int id)
    {
        slotButtons[currentPlayerSelectingClass].Text = "Slot " + (currentPlayerSelectingClass + 1);
        slotButtons[currentPlayerSelectingClass].ButtonStyle.normal.textColor = Color.white;

        RemovePlayer(Players[id]);

        currentPlayerSelectingClass = id;
    }

    public GameObject playerPrefab;
    public GameObject cameraPrefab;
    public PlayerClass[] PlayerClasses;

    public GameObject GameContainer;

    public void SelectClass(int id)
    {
        GameObject newPlayer = (GameObject)Object.Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
        newPlayer.SetActive(false);
        Players[currentPlayerSelectingClass] = newPlayer.GetComponent<PlayerController>();

        GameObject newCam = (GameObject) Object.Instantiate(cameraPrefab, new Vector3(0,0,-10), Quaternion.identity);
        newCam.SetActive(false);

        newCam.GetComponent<PlayerUI>().playerControl = Players[currentPlayerSelectingClass];

        Cameras[currentPlayerSelectingClass] = newCam.GetComponent<CameraController>();
        Cameras[currentPlayerSelectingClass].player = newPlayer.transform;
        Cameras[currentPlayerSelectingClass].playerControl = newPlayer.GetComponent<PlayerController>();

        Players[currentPlayerSelectingClass].PlayerId = currentPlayerSelectingClass;

        PlayerClass playerClass = (PlayerClass) Object.Instantiate(PlayerClasses[id]);
        playerClass.transform.parent = newPlayer.transform;
        Players[currentPlayerSelectingClass].PlayerClass = playerClass;

        slotButtons[currentPlayerSelectingClass].Text = "Selected: " + Players[currentPlayerSelectingClass].PlayerClass.name.Replace("(Clone)", "");
        slotButtons[currentPlayerSelectingClass].ButtonStyle.normal.textColor = Color.green;


        currentPlayerSelectingClass = -1;
        for (int i = 0; i < 4; i++)
        {
            if (Players[i] == null)
            {
                currentPlayerSelectingClass = i;
                break;
            }
        }
    }

    public bool GamePaused = false;

    public static Vector3 GetSpawnPosition()
    {
        return Instance.SpawnPosition.position;
    }

    public void OnPause()
    {
        GamePaused = true;
    }

    public void OnResume()
    {
        GamePaused = false;
    }

    public void AddPlayer(PlayerController playerController)
    {
        
        if (Players.Contains(playerController))
        {
            Debug.Log("Containing");
            return;
        }

        for (int i = 0; i < Players.Length; i++)
        {
            if (Players[i] == null)
            {
                Debug.Log("Player Added");
                Players[i] = playerController;
                return;
            }
        }
        Debug.Log("Not addded");
    }
}
