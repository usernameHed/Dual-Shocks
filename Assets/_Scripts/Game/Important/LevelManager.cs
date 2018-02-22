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
    /// </summary>
    private void SpawnPlayer()
    {
        Debug.Log("ici Spawn les players");

    }

    #endregion

    #region Unity ending functions

    #endregion
}
