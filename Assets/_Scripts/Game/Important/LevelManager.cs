using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using System;
using System.Collections.Generic;

/// <summary>
/// LevelManager Description
/// </summary>
public class LevelManager : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("Objects"), Tooltip("list des players local du jeu !"), SerializeField]
    private List<GameObject> playersLocal;
    public List<GameObject> PlayersLocal { get { return playersLocal; } }

    [FoldoutGroup("Objects"), Tooltip("player parent"), SerializeField]
    private Transform playerParent;

    [FoldoutGroup("Debug"), Tooltip("gère la GUI in games"), SerializeField]
    private DisplayInGame displayInGame;
    #endregion

    #region Initialization

    private void Awake()
    {
        GameManager.GetSingleton.LevelManager = this;
    }

    /// <summary>
    /// appelé depuis le gameManager quand tout semble bon
    /// </summary>
    public void StartGame()
    {
        SpawnPlayer();
        displayInGame.InitDisplay();
        displayInGame.ChangeDisplayInGame();
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


            playersLocal[i].SetActive(playerBallInit.PlayerData[i].active);
        }
    }

    #endregion

    #region Unity ending functions

    #endregion
}
