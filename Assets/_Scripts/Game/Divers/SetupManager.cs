using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// MenuManager Description
/// </summary>
public class SetupManager : MonoBehaviour, ILevelManager
{
    #region Attributes
    [FoldoutGroup("GamePlay"), OnValueChanged("ChangePhase", true), Tooltip("Debug"), SerializeField]
    private int[] idPhaseConnexion = new int[4];
    public int[] IdPhaseConnexion { get { return (idPhaseConnexion); } }

    [FoldoutGroup("Debug"), Tooltip("Debug"), SerializeField]
    private DisplayInSetup displayInSetup;
    public DisplayInSetup DisplayInSetupScript { get { return displayInSetup; } }

    [FoldoutGroup("Debug"), Tooltip("gere le temps avant de pouvoir faire Restart"), SerializeField]
    private FrequencyTimer coolDownRestart;

    private PlayerConnected playerConnected;
    private bool enabledScript = true;

    #endregion

    #region Initialization

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.AdditiveJustFinishLoad, CalledWhenLevelLoaded);
    }

    private void Awake()
    {
        coolDownRestart.Ready();
        enabledScript = true;
    }

    private void Start()
    {
        playerConnected = PlayerConnected.Instance;
    }

    /// <summary>
    /// est appelé depuis le GameManager depuis l'interface
    /// à l'initialisation...
    /// </summary>
    public void InitScene()
    {
        if (GameManager.GetSingleton.FromGame)
        {
            GameManager.GetSingleton.FromGame = false;
            SetupPhaseThenFromMenu();
        }
            
        ChangePhase();
        displayInSetup.InitDisplay();
    }
    /// <summary>
    /// est appelé depuis le GameManager depuis l'interface
    /// est appelé quand il y a un changement de gamePad
    /// </summary>
    public void CallGamePad()
    {
        ChangePhase();
    }

    /// <summary>
    /// est appelé par les event quand une scene additive a été chargé
    /// </summary>
    private void CalledWhenLevelLoaded()
    {
        LevelDesign.GetSingleton.HideContent();
    }

    /// <summary>
    /// est appelé lorsqu'on passe du game au menu
    /// </summary>
    private void SetupPhaseThenFromMenu()
    {
        for (int i = 0; i < idPhaseConnexion.Length; i++)
        {
            if (GameManager.GetSingleton.PlayerBallInit.PlayerData[i].active)
                idPhaseConnexion[i] = 2;
        }
    }

    /// <summary>
    /// ici change de phase (selon les input, connexion de manette)
    /// </summary>
    public void ChangePhase()
    {
        if (!playerConnected)
            playerConnected = PlayerConnected.Instance;

        for (int i = 0; i < idPhaseConnexion.Length; i++)
        {
            switch(idPhaseConnexion[i])
            {
                case 0: //ici passe en phase 2 dès le début, si les manettes sont connecté
                    Debug.Log("ici phase 0");
                    if (playerConnected.playerArrayConnected[i])
                        idPhaseConnexion[i] = 1;

                    break;
                case 1: //à partir d'ici, revient en phase 0 si les manette se déconnecte
                    Debug.Log("ici phase 1");
                    if (!playerConnected.playerArrayConnected[i])
                        idPhaseConnexion[i] = 0;

                    break;
                case 2:
                    Debug.Log("ici phase 2");
                    if (!playerConnected.playerArrayConnected[i])
                        idPhaseConnexion[i] = 0;
                    break;
                case 3:
                    if (!playerConnected.playerArrayConnected[i])
                        idPhaseConnexion[i] = 0;

                    Debug.Log("ici phase 3");
                    break;

            }
        }
        IsReadyToPlay();
        displayInSetup.ChangeDisplayInGame();
    }

    #endregion

    #region Core
    /// <summary>
    /// le jeux est-il prêt à être joué ?
    /// </summary>
    public bool IsReadyToPlay()
    {
        int players = 0;
        for (int i = 0; i < idPhaseConnexion.Length; i++)
        {
            //si il y a un joueur de connecté, qui a passé la phase "press A", alors il joue obligatoirement
            //si celui-ci n'est pas validé, il n'y a pas encore de jeu...
            if (idPhaseConnexion[i] > 1 && idPhaseConnexion[i] < 3)
            {
                displayInSetup.Play.SetActive(false);
                return (false);
            }
                
            //active le joueur qui a validé
            if (idPhaseConnexion[i] >= 3)
            {
                players++;
            }
        }
        //s'il n'y a aucune joueur qui a validé, ne pas lancer !
        if (players < 1)
        {
            displayInSetup.Play.SetActive(false);
            return (false);
        }
        displayInSetup.Play.SetActive(true);
        return (true);
    }

    /// <summary>
    /// On lance le jeu ou pas ?
    /// </summary>
    private bool ReadyToPlay()
    {
        //si les joueurs ne sont pas encore prêt...
        if (!IsReadyToPlay())
            return (false);

        //ici active les joueurs qui ont validé... et désactive les autres
        for (int i = 0; i < idPhaseConnexion.Length; i++)
        {
            GameManager.GetSingleton.PlayerBallInit.PlayerData[i].active = (idPhaseConnexion[i] >= 3);
        }

        Play(); //ici play enfin !!!!!
        return (true);
    }

    /// <summary>
    /// input des 4 joueurs
    /// </summary>
    private void InputPlayer()
    {
        for (int i = 0; i < idPhaseConnexion.Length; i++)
        {
            if (PlayerConnected.Instance.getPlayer(i).GetButtonDown("FireA") && idPhaseConnexion[i] >= 1 && idPhaseConnexion[i] < 4)
            {
                idPhaseConnexion[i] += 1;

                if (idPhaseConnexion[i] == 4)
                {
                    idPhaseConnexion[i] = 3;

                    if (ReadyToPlay())
                        return;
                }
                ChangePhase();
            }
            if (    (
                    PlayerConnected.Instance.getPlayer(i).GetButtonDown("FireB")
                        ||
                    (PlayerConnected.Instance.getPlayer(-1).GetButtonDown("Escape") && PlayerConnected.Instance.getNbPlayer() == 1)
                    )
                        && idPhaseConnexion[i] > 0)
            {
                idPhaseConnexion[i] -= 1;
                ChangePhase();
            }
        }
    }

    private void InputGame()
    {
        //Si: on appui sur echape et qu'il y a 2+ player de connecté, on quit
        //SI: on appui sur echap, qu'il n'y a qu'un joueur (clavier), et qu'il est en phase 0, 1, on quit
        //SI: n'importe qui appuis sur Back avec le gamepad, on quit.
        if ((PlayerConnected.Instance.getPlayer(-1).GetButtonDown("Escape") && PlayerConnected.Instance.getNbPlayer() > 1)
            || (PlayerConnected.Instance.getPlayer(-1).GetButtonDown("Escape") && PlayerConnected.Instance.getNbPlayer() == 1 && idPhaseConnexion[0] <= 1)
            || PlayerConnected.Instance.getButtonDownFromAnyGamePad("Back"))
        {
            Quit();
        }
    }

    /// <summary>
    /// ici lance le jeu, il est chargé !
    /// </summary>
    [Button("Play")]
    private void Play()
    {
        if (!enabledScript)
            return;
        if (!coolDownRestart.Ready())
            return;

        enabledScript = false;
        displayInSetup.CleanUp();
        GameManager.GetSingleton.RestartGame(false);    //notification spécial ... pas forcément utile
        GameManager.GetSingleton.SceneManagerLocal.PlayNext();
    }

    [Button("Quit")]
    private void Quit()
    {
        GameManager.GetSingleton.SceneManagerLocal.PlayPrevious();
    }
    #endregion

    #region Unity ending functions
    private void Update()
    {
        InputPlayer();
        InputGame();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.AdditiveJustFinishLoad, CalledWhenLevelLoaded);
    }
    #endregion
}
