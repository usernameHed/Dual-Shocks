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
    [FoldoutGroup("Objetcs"), Tooltip("Debug"), SerializeField]
    private List<Rope> rope;

    [FoldoutGroup("Scene"), Tooltip("Debug"), SerializeField]
    private string sceneToLoad = "3_Game";
    [FoldoutGroup("Scene"), Tooltip("Debug"), SerializeField]
    private string scenePrevious = "1_Menu";
    [FoldoutGroup("Scene"), Tooltip("Debug"), SerializeField]
    private float speedTransition = 0.5f;

    #endregion

    #region Initialization

    private void Start()
    {
        GameManager.GetSingleton.SetupManagerScript = this;
        SceneChangeManager.GetSingleton.StartLoading(sceneToLoad, false);
        InitSetup();
    }

    /// <summary>
    /// initialise les setups...
    /// </summary>
    private void InitSetup()
    {
        /*for (int i = 0; i < rope.Count; i++)
        {
            rope[i].InitPhysicRope();
        }*/
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
