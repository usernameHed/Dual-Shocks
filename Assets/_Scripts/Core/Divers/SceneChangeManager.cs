using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneChangeManager : MonoBehaviour
{
	#region Attributes
    /// <summary>
    /// variable privé
    /// </summary>
    private static SceneChangeManager instance;
    public static SceneChangeManager GetSingleton
    {
        get { return instance; }
    }

    private AsyncOperation async;                   //gestion de quitter de manière asynchrone


    private bool isCharging = false;             //détermine si une scène est en chargement

    #endregion

    #region Initialization
    /// <summary>
    /// test si on met le script en UNIQUE
    /// </summary>
    public void SetSingleton()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    /// <summary>
    /// Initialisation
    /// </summary>
    private void Awake()                                                    //initialisation referencce
    {
        SetSingleton();
    }

    #endregion

    #region Core

    ///////////////////////////////////////////////////////////////////////////// gestion asyncrone
    /// <summary>
    /// charge une scène en mode async
    /// CAD on la charge en mémoire, mais on fait autre chose en attendant...
    /// (écran de chargement !)
    /// </summary>
    [ContextMenu("StartLoading Async")]
    public void StartLoading(string scene = "Game", bool swapWhenFinish = true)
    {
        StartCoroutine(load(scene, swapWhenFinish));
    }

    /// <summary>
    /// Ici charge la scène en asyncrone...
    /// </summary>
    IEnumerator load(string scene, bool swapWhenFinish)
    {
        Debug.LogWarning("ASYNC LOAD STARTED - " +
           "DO NOT EXIT PLAY MODE UNTIL SCENE LOADS... UNITY WILL CRASH");
		async = SceneManager.LoadSceneAsync(scene/*, LoadSceneMode.Additive*/);
        async.allowSceneActivation = swapWhenFinish;
        isCharging = true;
        yield return async;
        if (swapWhenFinish)
            ActivateScene();
    }

    /// <summary>
    /// ICI appelle l'activation de la scène précédamment chargé
    /// </summary>
    public void ActivateScene()
    {
        if (!isCharging)
            return;
        isCharging = false;
        async.allowSceneActivation = true;
    }

    //////////////////////////////////////////////////////////////////////////////// transition scenes
    /// <summary>
    /// jump à une scène
    /// </summary>
    [ContextMenu("JumpToScene")]
    public void JumpToScene(string scene = "")
    {
        if (scene == "")
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            return;
        }
        SceneManager.LoadScene(scene);
    }

    /// <summary>
    /// ajoute une scène à celle courrante
    /// </summary>
    [ContextMenu("JumpAdditiveScene")]
    public void JumpAdditiveScene(string scene = "Game")
    {
        SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        //SceneManager.UnloadSceneAsync("Game");
    }


    /// <summary>
    /// Change de scène avec un fade
    /// </summary>
    /// <param name="scene"></param>
    [ContextMenu("JumpToSceneWithFade")]
    public void JumpToSceneWithFade(string scene = "")
    {
        if (scene == "")
            scene = SceneManager.GetActiveScene().name;

        if (!gameObject.GetComponent<Fading>().enabled)
        {
            gameObject.GetComponent<Fading>().enabled = true;
        }

        StartCoroutine(JumpToSceneWithFadeWait(scene));
    }

    IEnumerator JumpToSceneWithFadeWait(string scene = "Game")
    {
        float fadeTime = gameObject.GetComponent<Fading>().BeginFade(1);
        yield return new WaitForSeconds(fadeTime);
        JumpToScene(scene);
    }

    /// <summary>
    /// quite le jeu (si on est dans l'éditeur, quite le mode play)
    /// </summary>
    [ContextMenu("Quit")]
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion
}