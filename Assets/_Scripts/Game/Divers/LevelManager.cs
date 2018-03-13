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

    [FoldoutGroup("Debug"), Tooltip("gere le temps avant de pouvoir faire Restart"), SerializeField]
    private FrequencyTimer coolDownRestart;

    private int playerAlive = 0;
    private bool roundIsOver = false;
    private bool enabledScript = true;

    private bool partyOver = false;
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
        roundIsOver = false;
        playerAlive = 0;
        partyOver = false;

        LevelInit();
    }

    /// <summary>
    /// est appelé à l'init, OU au restart
    /// </summary>
    private void LevelInit()
    {
        if (!roundIsOver)
        {
            GameStart();            //première fois que les partie on débuté
            return;
        }
        GameContinue();           //ici on a déja commencé, on en est au round X
    }

    /// <summary>
    /// le jeu commence la première fois
    /// </summary>
    private void GameStart()
    {
        Debug.Log("ici reset les scores... en début de jeu");
        ScoreManager.Instance.ResetAll();   //reset les scores

        EventManager.TriggerEvent(GameData.Event.GameStart);

        RoundStart();
    }

    private void GameContinue()
    {
        roundIsOver = false;
        Debug.Log("ici on active des truck fifou pour le restart ?");
        RoundStart();
    }

    /// <summary>
    /// le round start
    /// </summary>
    private void RoundStart()
    {
        Debug.Log("ici start round !");
        ScoreManager.Instance.NextRound();
        EventManager.TriggerEvent(GameData.Event.RoundStart);

        //init le level design !! (+ spawn les entité dans le jeu)
        LevelDesign.GetSingleton.InitLevelDesign();

        SpawnPlayer();
    }

    /// <summary>
    /// est appelé quand tout les joueurs sont mort / le dernier en vie
    /// </summary>
    private void RoundOver()
    {
        roundIsOver = true;
        EventManager.TriggerEvent(GameData.Event.GameOver);
        if (ScoreManager.Instance.IsPartyOver())
            PartyOver();
    }

    /// <summary>
    /// ici la partie est fini !
    /// </summary>
    private void PartyOver()
    {
        Debug.Log("ici la partie est fini !!!");
        partyOver = true;
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
            Transform posFirstBall = LevelDesign.GetSingleton.SpawnPlayersGet[i].spawnBall[0];
            Transform posSecondBall = LevelDesign.GetSingleton.SpawnPlayersGet[i].spawnBall[1];

            playerController.SpawnBallPos(posFirstBall, posSecondBall);
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
            RoundOver();
        }
    }

    /// <summary>
    /// est appelé par les joueurs à leurs mort...
    /// </summary>
    private bool IsGameOver()
    {
        if (playerAlive <= 1)
        {
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
        if (roundIsOver && !partyOver)
        {
            Debug.Log("ici restart ?");
            for (int i = 0; i < playersLocal.Count; i++)
            {
                playersLocal[i].SetActive(false);
            }
            ObjectsPooler.Instance.desactiveEveryOneForTransition();
            ObjectsPoolerLocal.Instance.desactiveEveryOneForTransition();
            LevelInit();
        }
        else if (roundIsOver && partyOver)
        {
            Quit();
        }

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

        //ObjectsPooler.Instance.desactiveEveryOneForTransition();
        //ObjectsPoolerLocal.Instance.desactiveEveryOneForTransition();

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
