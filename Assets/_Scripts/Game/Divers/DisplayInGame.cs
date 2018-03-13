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
        //public GameObject objectSpawn;        //a désactiver si on joue pas
        public GameObject displaySpawn;        //a désactiver si on joue pas
        public TextMeshProUGUI textScore;
        //public List<Transform> spawnBall;
        public bool active;
    }

    #region Attributes
    [FoldoutGroup("Objects"), Tooltip("test du round"), SerializeField]
    private TextMeshProUGUI textRound;

    [FoldoutGroup("Objects"), Tooltip("list spawn players"), SerializeField]
    private PlayerRef[] playerRocks = new PlayerRef[playerMax];
    public PlayerRef[] PlayerRocks { get { return (playerRocks); } }

    [FoldoutGroup("Objects"), Tooltip("display round over"), SerializeField]
    private GameObject roundOver;
    [FoldoutGroup("Objects"), Tooltip("display party over"), SerializeField]
    private GameObject partyOver;


    private const int playerMax = 4;  //nombre de ball du joueur
    private PlayerData data;

    #endregion

    #region Initialization
    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.GameStart, GameStart);
        EventManager.StartListening(GameData.Event.RoundStart, RoundStart);
        EventManager.StartListening(GameData.Event.GameOver, GameOver);
    }
    #endregion

    #region Core
    /// <summary>
    /// initialise le display avec les joueurs connecté
    /// </summary>
    private void GameStart()
    {
        partyOver.SetActive(false);

        data = ScoreManager.Instance.Data;
        for (int i = 0; i < playerRocks.Length; i++)
        {
            playerRocks[i].active = GameManager.GetSingleton.PlayerBallInit.PlayerData[i].active;   //ici active ou non selon si le player est connecté
            playerRocks[i].displaySpawn.SetActive(playerRocks[i].active);
        }
    }

    /// <summary>
    /// actualise le display par rapport aux scores, player, etc
    /// </summary>
    private void RoundStart()
    {
        roundOver.SetActive(false);

        textRound.text = data.CurrentRound.ToString();
        for (int i = 0; i < playerRocks.Length; i++)
        {
            if (playerRocks[i].active)
            {
                playerRocks[i].textScore.text = data.ScorePlayer[i].ToString();
            }
        }
    }

    /// <summary>
    /// appelé quand la game est over
    /// </summary>
    private void GameOver()
    {
        if (ScoreManager.Instance.IsPartyOver())
        {
            Debug.Log("ici C'est la fin de la partie ! affiche l'écran de victoire");
            partyOver.SetActive(true);
        }
        else
        {
            Debug.Log("ici c'est la fin du round" + ScoreManager.Instance.Data.CurrentRound);
            roundOver.SetActive(true);
        }
    }

    #endregion

    #region Unity ending functions
    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameStart, GameStart);
        EventManager.StopListening(GameData.Event.RoundStart, RoundStart);
        EventManager.StopListening(GameData.Event.GameOver, GameOver);
    }
    #endregion
}
