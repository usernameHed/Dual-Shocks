using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using System;
using System.Collections.Generic;

/// <summary>
/// LevelManager Description
/// </summary>
public class LevelManager : MonoBehaviour, ILevelManager
{
    #region Attributes
    [FoldoutGroup("Objects"), Tooltip("list des players local du jeu !"), SerializeField]
    private List<GameObject> playersLocal;
    public List<GameObject> PlayersLocal { get { return playersLocal; } }

    [FoldoutGroup("Objects"), Tooltip("player parent"), SerializeField]
    private Transform playerParent;

    [FoldoutGroup("Debug"), Tooltip("gère la GUI in games"), SerializeField]
    private DisplayInGame displayInGame;

    [FoldoutGroup("Debug"), Tooltip("gere le temps avant de pouvoir faire Restart"), SerializeField]
    private FrequencyTimer coolDownRestart;

    private int playerAlive = 0;

    private bool enabledScript = true;
    #endregion

    #region Initialization

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.PlayerDeath, PlayerDeath);
    }

    private void Awake()
    {
        coolDownRestart.Ready();
        enabledScript = true;
    }

    /// <summary>
    /// est appelé depuis le GameManager depuis l'interface
    /// à l'initialisation...
    /// </summary>
    public void InitScene()
    {
        LevelDesign.GetSingleton.InitLevelDesign(); //init le level design !!

        EventManager.TriggerEvent(GameData.Event.RoundStart);

        Debug.Log("ici reset les scores... (seulement quand on fait un nouveau round ??)");
        ScoreManager.Instance.ResetAll();   //reset les scores
        //GameManager.GetSingleton.PlayerBallInit.Setup();

        SpawnPlayer();
        displayInGame.InitDisplay();
        displayInGame.ChangeDisplayInGame();
    }
    /// <summary>
    /// est appelé depuis le GameManager depuis l'interface
    /// est appelé quand il y a un changement de gamePad
    /// </summary>
    public void CallGamePad()
    {
        
    }
    #endregion

    #region Core

    /// <summary>
    /// ici créé les players
    /// (ou plutot active les bon player en leurs donnan
    /// </summary>
    private void SpawnPlayer()
    {
        Debug.Log("ici Spawn les players");
        PlayerBallInit playerBallInit = GameManager.GetSingleton.PlayerBallInit;

        //ici parcours les player locals, et change leurs ballInfo avec ceux du playerBallInit !
        for (int i = 0; i < playerBallInit.PlayerData.Length; i++)
        {
            PlayerController playerController = playersLocal[i].GetComponent<PlayerController>();
            playerController.IdPlayer = playerBallInit.PlayerData[i].idPlayer;
            playerController.BallInfo = playerBallInit.PlayerData[i].ballInfo;

            
            
            bool activePlayer = playerBallInit.PlayerData[i].active;
            playerController.SpawnBallPos(displayInGame.PlayerRocks[i].spawnBall[0], displayInGame.PlayerRocks[i].spawnBall[1]);
            if (activePlayer)
            {
                playerAlive++;
            }
            playersLocal[i].SetActive(activePlayer);
        }
    }

    private void InputGame()
    {
        if (PlayerConnected.Instance.getPlayer(-1).GetButtonDown("Escape")
            || PlayerConnected.Instance.getButtonDownFromAnyGamePad("Back"))
        {
            Quit();
        }
        if (PlayerConnected.Instance.getPlayer(-1).GetButtonDown("Restart")
            || PlayerConnected.Instance.getButtonDownFromAnyGamePad("Restart"))
        {
            Restart();
        }
    }

    [SerializeField]
    private void PlayerDeath(int idPlayer)
    {
        Debug.Log("un player est mort !");
        playerAlive--;
        if (IsGameOver())
        {
            Debug.Log("ici c'est la fin du jeu !!! Setup les rounds");
        }

    }

    /// <summary>
    /// est appelé par les joueurs à leurs mort...
    /// </summary>
    private bool IsGameOver()
    {
        if (playerAlive <= 1)
        {
            EventManager.TriggerEvent(GameData.Event.GameOver);
            return (true);
        }
        return (false);
    }

    /// <summary>
    /// restart le jeu
    /// </summary>
    [Button("Restart")]
    public void Restart()
    {
        

        Debug.Log("Ici on ne restart plus la scene comme ça...");
        //GameManager.GetSingleton.RestartGame(true);
        //GameManager.GetSingleton.SceneManagerLocal.PlayNext();
    }

    [Button("Quit")]
    public void Quit()
    {
        if (!enabledScript)
            return;
        if (!coolDownRestart.Ready())
            return;

        enabledScript = false;
        LevelDesign.GetSingleton.DesactiveScene(); //desactive le level design !!
        GameManager.GetSingleton.FromGameToSetup(true);
        GameManager.GetSingleton.SceneManagerLocal.PlayPrevious(false);
    }

    #endregion

    #region Unity ending functions
    private void Update()
    {
        InputGame();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.PlayerDeath, PlayerDeath);
    }
    #endregion
}
