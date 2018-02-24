using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// MenuManager Description
/// </summary>
public class SetupManager : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), OnValueChanged("ChangePhase", true), Tooltip("Debug"), SerializeField]
    private int[] idPhaseConnexion = new int[4];


    [FoldoutGroup("Objetcs"), Tooltip("Debug"), SerializeField]
    private List<Rope> rope;

    [FoldoutGroup("Scene"), Tooltip("Debug"), SerializeField]
    private string sceneToLoad = "3_Game";
    [FoldoutGroup("Scene"), Tooltip("Debug"), SerializeField]
    private string scenePrevious = "1_Menu";
    [FoldoutGroup("Scene"), Tooltip("Debug"), SerializeField]
    private float speedTransition = 0.5f;

    private PlayerConnected playerConnected;

    #endregion

    #region Initialization

    private void Start()
    {
        GameManager.GetSingleton.SetupManagerScript = this;
        SceneChangeManager.GetSingleton.StartLoading(sceneToLoad, false);
        playerConnected = PlayerConnected.GetSingleton;
    }

    /// <summary>
    /// initialise les setups...
    /// </summary>
    public void InitSetup()
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
                    Debug.Log("ici phase 3");
                    if (!playerConnected.playerArrayConnected[i])
                        idPhaseConnexion[i] = 0;

                    break;

            }
        }
    }

    /// <summary>
    /// supprime les Link (les remet dans la pool !) pour la suite...
    /// </summary>
    private void CleanUp()
    {
        for (int i = 0; i < rope.Count; i++)
        {
            rope[i].ClearJoints();
        }
    }
    #endregion

    #region Core
    /// <summary>
    /// ici lance le jeu, il est chargé !
    /// </summary>
    [Button("Play")]
    public void Play()
    {
        CleanUp();
        GameManager.GetSingleton.RestartGame(false);
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
