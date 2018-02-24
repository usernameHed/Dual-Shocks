using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

[Serializable]
public struct DataPlayers
{
    public int idPlayer;
    public PlayerBall[] ballInfo;
    public bool active;
    public bool gamepadActive;

    DataPlayers(int _idPlayer, PlayerBall[] _ballInfo, bool _active, bool _gamepadActive)
    {
        idPlayer = _idPlayer;
        ballInfo = _ballInfo;
        active = _active;
        gamepadActive = _gamepadActive;
    }
}

/// <summary>
/// PlayerBallInit Description
/// </summary>
public class PlayerBallInit : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("Important, défini si on vient du level setup, ou du play !"), SerializeField]
    private bool fromLevel = false;
    public bool FromLevel { set { fromLevel = value; } }

    [FoldoutGroup("GamePlay"), Tooltip("les infos des 2 balls à créé sur le player + ses weapons"), SerializeField]
    //private PlayerBall[] playerBallInfo = new PlayerBall[2];
    private DataPlayers[] playerData = new DataPlayers[4];
    public DataPlayers[] PlayerData { get { return playerData; } }

    #endregion

    #region Initialization


    #endregion

    #region Core
    /// <summary>
    /// ici setup les donnée du joueurs, 
    /// </summary>
    public void Setup()
    {
        if (!fromLevel)
            SetPlayerActiveWithGamePadConnexion();
        SetupGamePadActive();
    }

    /// <summary>
    /// ici appelé quand les changement de manette se passe...
    /// </summary>
    public void SetupGamePadActive()
    {
        for (int i = 0; i < PlayerConnected.GetSingleton.playerArrayConnected.Length; i++)
        {
            playerData[i].gamepadActive = PlayerConnected.GetSingleton.playerArrayConnected[i];
        }
    }

    /// <summary>
    /// normalement ils ont déjà été setup dans la scene précédente,
    /// mais si !fromLevel, alors récupérer les donnée de PlayerConnected
    /// pour set les players qui joue
    /// </summary>
    private void SetPlayerActiveWithGamePadConnexion()
    {
        for (int i = 0; i < PlayerConnected.GetSingleton.playerArrayConnected.Length; i++)
        {
            playerData[i].active = PlayerConnected.GetSingleton.playerArrayConnected[i];
        }
    }
    #endregion

    #region Unity ending functions

    #endregion
}
