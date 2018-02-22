using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// MenuManager Description
/// </summary>
public class MenuManager : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("Debug"), SerializeField]
    private string sceneToLoad = "2_Setup";

    [FoldoutGroup("Objects"), Tooltip("Debug"), SerializeField]
    private List<Button> buttonsMainMenu;

    private bool enabledScript = true;
    #endregion

    #region Initialization

    private void Start()
    {
        enabledScript = true;
        GameManager.GetSingleton.MenuManagerScript = this;
        SceneChangeManager.GetSingleton.StartLoading(sceneToLoad, false);
    }
    #endregion

    #region Core
    /// <summary>
    /// ici lance le jeu, il est chargé !
    /// </summary>
    [Button("Play")]
    public void Play()
    {
        SceneChangeManager.GetSingleton.ActivateSceneWithFade();
    }

    [Button("Quit")]
    public void Quit()
    {
        enabledScript = false;
        buttonsMainMenu[1].Select();
        
        SceneChangeManager.GetSingleton.QuitFade();
    }

    /// <summary>
    /// est appelé pour débug le clique
    /// Quand on clique avec la souris: reselect le premier bouton !
    /// </summary>
    private void DebugMouseCLick()
    {
        if (!enabledScript)
            return;
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
        {
            buttonsMainMenu[0].Select();
        }
    }
    #endregion

    #region Unity ending functions
    private void Update()
    {
        if (!enabledScript)
            return;
        DebugMouseCLick();
    }
    #endregion
}
