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
    #endregion

    #region Initialization

    private void Awake()
    {
        coolDownRestart.Ready();
    }

    /// <summary>
    /// est appelé depuis le GameManager depuis l'interface
    /// à l'initialisation...
    /// </summary>
    public void InitScene()
    {
        ScoreManager.GetSingleton.ResetAll();   //reset les scores
        GameManager.GetSingleton.PlayerBallInit.Setup();

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
            /*if (activePlayer)
            {
                Debug.Log("ICI ) la position des spawn STP !!!");
                playerController.FollowersList[0].position = displayInGame.PlayerRocks[i].spawnBall[0].position;
                playerController.FollowersList[1].position = displayInGame.PlayerRocks[i].spawnBall[1].position;

            }*/
            playersLocal[i].SetActive(activePlayer);
        }
    }

    private void InputGame()
    {
        if (PlayerConnected.GetSingleton.getPlayer(-1).GetButtonDown("Escape")
            || PlayerConnected.GetSingleton.getButtonDownFromAnyGamePad("Back"))
        {
            Quit();
        }
        if (PlayerConnected.GetSingleton.getPlayer(-1).GetButtonDown("Restart")
            || PlayerConnected.GetSingleton.getButtonDownFromAnyGamePad("Restart"))
        {
            Restart();
        }
    }

    /// <summary>
    /// restart le jeu
    /// </summary>
    [Button("Restart")]
    public void Restart()
    {
        if (!coolDownRestart.Ready())
            return;

        GameManager.GetSingleton.RestartGame(true);
        GameManager.GetSingleton.SceneManagerLocal.PlayNext();
    }

    [Button("Quit")]
    public void Quit()
    {
        GameManager.GetSingleton.SceneManagerLocal.PlayPrevious();
    }

    #endregion

    #region Unity ending functions
    private void Update()
    {
        InputGame();
    }
    #endregion
}
