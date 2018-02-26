using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// MenuManager Description
/// </summary>
[RequireComponent(typeof(DisplayInSetup))]
public class SetupManager : MonoBehaviour, ILevelManager
{
    #region Attributes
    [FoldoutGroup("GamePlay"), OnValueChanged("ChangePhase", true), Tooltip("Debug"), SerializeField]
    private int[] idPhaseConnexion = new int[4];
    public int[] IdPhaseConnexion { get { return (idPhaseConnexion); } }

    [FoldoutGroup("Debug"), Tooltip("Debug"), SerializeField]
    private DisplayInSetup displayInSetup;
    public DisplayInSetup DisplayInSetupScript { get { return displayInSetup; } }

    private PlayerConnected playerConnected;

    #endregion

    #region Initialization

    private void Start()
    {
        playerConnected = PlayerConnected.GetSingleton;
    }

    /// <summary>
    /// est appelé depuis le GameManager depuis l'interface
    /// à l'initialisation...
    /// </summary>
    public void InitScene()
    {
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
    /// ici change de phase (selon les input, connexion de manette)
    /// </summary>
    public void ChangePhase()
    {
        if (!playerConnected)
            playerConnected = PlayerConnected.GetSingleton;

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
        displayInSetup.ChangeDisplayInGame();
    }

    #endregion

    #region Core
    /// <summary>
    /// On lance le jeu ou pas ?
    /// </summary>
    private bool ReadyToPlay()
    {
        for (int i = 0; i < idPhaseConnexion.Length; i++)
        {
            if (idPhaseConnexion[i] > 0 && idPhaseConnexion[i] < 3)
                return (false);
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
            if (PlayerConnected.GetSingleton.getPlayer(i).GetButtonDown("FireA") && idPhaseConnexion[i] >= 1 && idPhaseConnexion[i] < 4)
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
            if (PlayerConnected.GetSingleton.getPlayer(i).GetButtonDown("FireB") && idPhaseConnexion[i] > 0)
            {
                idPhaseConnexion[i] -= 1;
                ChangePhase();
            }
        }
    }

    private void InputGame()
    {
        if (PlayerConnected.GetSingleton.getPlayer(-1).GetButtonDown("Escape")
            || PlayerConnected.GetSingleton.getPlayer(0).GetButtonDown("Back"))
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
    #endregion
}
