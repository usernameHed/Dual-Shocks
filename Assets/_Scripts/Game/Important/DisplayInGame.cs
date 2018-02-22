using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using System;
using System.Collections.Generic;

/// <summary>
/// DisplayInGame Description
/// </summary>
public class DisplayInGame : MonoBehaviour
{
    [Serializable]
    public struct PlayerRef
    {
        [Space(10)]
        public GameObject objectSpawn;        //a désactiver si on joue pas
        public GameObject displaySpawn;        //a désactiver si on joue pas
        public TextMeshProUGUI textScore;
        public List<Transform> spawnBall;
        public bool active;
    }

    #region Attributes
    [FoldoutGroup("Objects"), Tooltip("test du round"), SerializeField]
    private TextMeshProUGUI textRound;

    [FoldoutGroup("Objects"), Tooltip("list spawn players"), SerializeField]
    private PlayerRef[] playerRocks = new PlayerRef[playerMax];


    private const int playerMax = 4;  //nombre de ball du joueur
    private PlayerData data;

    #endregion

    #region Initialization

    #endregion

    #region Core
    /// <summary>
    /// initialise le display avec les joueurs connecté
    /// </summary>
    public void InitDisplay()
    {
        data = ScoreManager.GetSingleton.Data;
        for (int i = 0; i < playerRocks.Length; i++)
        {
            playerRocks[i].active = true;   //ici active ou non selon si le player est connecté
        }
    }

    /// <summary>
    /// actualise le display par rapport aux scores, player, etc
    /// </summary>
    public void ChangeDisplayInGame()
    {
        textRound.text = data.CurrentRound.ToString();
        for (int i = 0; i < playerRocks.Length; i++)
        {
            if (playerRocks[i].active)
            {
                playerRocks[i].textScore.text = data.ScorePlayer[i].ToString();
            }
        }

    }
    #endregion

    #region Unity ending functions

    #endregion
}
