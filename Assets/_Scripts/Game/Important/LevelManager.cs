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
    [FoldoutGroup("Scene"), Tooltip("Debug"), SerializeField]
    private string sceneToLoad = "3_Game";
    [FoldoutGroup("Scene"), Tooltip("Debug"), SerializeField]
    private string scenePrevious = "1_Menu";
    [FoldoutGroup("Scene"), Tooltip("Debug"), SerializeField]
    private float speedTransition = 0.5f;

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
        Invoke("ReplayGame", 3f);   //reload le game après X secondes
    }

    /// <summary>
    /// recharge la scene courante pour un meilleur restart !
    /// </summary>
    private void ReplayGame()
    {
        SceneChangeManager.GetSingleton.StartLoading(sceneToLoad, false);
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

    /// <summary>
    /// restart le jeu
    /// </summary>
    [Button("Restart")]
    public void Restart()
    {
        GameManager.GetSingleton.RestartGame(true);
        SceneChangeManager.GetSingleton.ActivateSceneWithFade(speedTransition);
    }

    [Button("Quit")]
    public void Quit()
    {
        SceneChangeManager.GetSingleton.UnloadScene(sceneToLoad);
        SceneChangeManager.GetSingleton.JumpToSceneWithFade(scenePrevious, speedTransition);
    }

    #endregion

    #region Unity ending functions

    #endregion
}
