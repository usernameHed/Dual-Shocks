﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

public class SceneManagerGlobal : MonoBehaviour
{
    #region Attributes

    private struct SceneCharging
    {
        public string scene;
        public AsyncOperation async;
        public bool isAdditive;
        public bool swapWhenFinishUpload;
    }
    /// <summary>
    /// variable 
    /// </summary>
    //[FoldoutGroup("Debug"), Tooltip("Scene to load at start"), SerializeField]
    private List<SceneCharging> sceneCharging = new List<SceneCharging>();
    //private AsyncOperation async;                   //gestion de quitter de manière asynchrone



    private static SceneManagerGlobal instance;
    public static SceneManagerGlobal GetSingleton
    {
        get { return instance; }
    }

    private bool closing = false;

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
    /// Ici ajoute une scene à charger
    /// </summary>
    /// <param name="scene">nom de la scène</param>
    /// <param name="swapWhenLoaded">est-ce qu'on change de scène dès qu'elle fini de charger ?</param>
    /// <param name="additive">est-ce qu'on ajoute la scène en additif ou pas ??</param>
    public void StartLoading(string scene, bool swapWhenLoaded = true, bool additive = false, bool fade = false, float speedFade = 1.0f)
    {
        if ((additive && fade) || scene == "")
        {
            Debug.LogError("pas possible");
            return;
        }

        SceneCharging sceneToCharge;
        ////////////////store scene to charge
        sceneToCharge.scene = scene;
        sceneToCharge.async = (!additive) ? SceneManager.LoadSceneAsync(scene)
                                         : SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);
        sceneToCharge.isAdditive = additive;
        sceneToCharge.swapWhenFinishUpload = swapWhenLoaded;
        sceneToCharge.async.allowSceneActivation = sceneToCharge.swapWhenFinishUpload;

        //ajoute la scène à charger...
        sceneCharging.Add(sceneToCharge);

        if (fade && sceneToCharge.swapWhenFinishUpload)
        {
            ActivateSceneWithFade(sceneCharging.Count - 1, speedFade);
            return;
        }

        if (sceneToCharge.swapWhenFinishUpload)
            StartCoroutine(SwapAfterLoad(sceneCharging.Count - 1));
    }

    /// <summary>
    /// ici on veut lancer une scène qui est / à été chargé ! on a son nom, on veut
    /// la chercher dans la liste et l'activer !
    /// </summary>
    public void ActivateScene(string scene, bool fade = false, float speedFade = 1f)
    {
        for (int i = 0; i < sceneCharging.Count; i++)
        {
            if (sceneCharging[i].scene == scene)
            {
                Debug.Log("Ou la ?");
                StartCoroutine(ActiveSceneWithFadeWait(i, speedFade));
                return;
            }
        }
        Debug.Log("scene not found !");
    }


    /// <summary>
    /// Ici active la scène demandé, si elle à été chargé !
    /// </summary>
    /// <param name="index">index de la scène de la list</param>
    /// <param name="justActive">ici on est sur que la scène est chargé !</param>
    private void ActivateScene(int index, bool restartIfNotCharged = false, float time = 0.5f)
    {
        /*if (!sceneCharging[index].async.isDone)
        {
            Debug.Log("charging....");
            if (restartIfNotCharged)
                StartCoroutine(WaitForActivateScene(index, time));
            return;
        }*/
        sceneCharging[index].async.allowSceneActivation = true;
        sceneCharging.RemoveAt(index);
    }
    //relance l'essai d'activation
    private IEnumerator WaitForActivateScene(int index, float time) { yield return new WaitForSeconds(time); ActivateScene(index, true, time);  }


    /// <summary>
    /// Ici Lance la scène [index] Dès qu'elle est chargé ! Qu'elle soit asyncro ou pas !
    /// </summary>
    private IEnumerator SwapAfterLoad(int index)
    {
        Debug.LogWarning("wait before switch... just wait");

        yield return sceneCharging[index].async;
        ActivateScene(index, true);
    }

    /// <summary>
    /// ici s'occupe de faire un fade, puis d'activer la scène ensuite
    /// </summary>
    private void ActivateSceneWithFade(int index, float speedFade)
    {
        Debug.Log("ici ??");
        StartCoroutine(ActiveSceneWithFadeWait(index, speedFade));
    }
    private IEnumerator ActiveSceneWithFadeWait(int index, float speedFade)
    {
        float fadeTime = gameObject.GetComponent<Fading>().BeginFade(1, speedFade);
        yield return new WaitForSeconds(fadeTime);
        Debug.Log("passe ici ??");
        ActivateScene(index, true); //essay d'activer, si on n'y arrive pas on réésai !!!
    }




    /// <summary>
    /// Unload une scene précédement loadé !
    /// </summary>
    public void UnloadScene(int index)
    {
        SceneManager.UnloadSceneAsync(sceneCharging[index].scene);
        sceneCharging.RemoveAt(index);
    }




    /// <summary>
    /// est appelé si on doit annuler le chargement d'une scene !
    /// </summary>
    public void UnloadScene(string scene)
    {
        SceneManager.UnloadSceneAsync(scene);
    }


    //////////////////////////////////////////////////////////////////////////////// transition scenes normal
    /// <summary>
    /// jump à une scène
    /// </summary>
    [ContextMenu("JumpToScene")]
    public void JumpToScene(string scene = "", bool fade = false)
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
    }
    [ContextMenu("JumpToSceneWithFade")]
    public void JumpToSceneWithFade(string scene, float speed = 1.5f)
    {
        StartCoroutine(JumpToSceneWithFadeWait(scene, speed));
    }
    private IEnumerator JumpToSceneWithFadeWait(string scene, float speed)
    {
        float fadeTime = gameObject.GetComponent<Fading>().BeginFade(1, speed);
        yield return new WaitForSeconds(speed / 2);
        JumpToScene(scene);
    }

    /// <summary>
    /// quit avec un fade !
    /// </summary>
    public void QuitGame(bool fade = false, float speed = 1.5f)
    {
        if (!fade)
        {
            Quit();
            return;
        }
        StartCoroutine(QuitWithFade(speed));
    }
    private IEnumerator QuitWithFade(float speed)
    {
        float fadeTime = gameObject.GetComponent<Fading>().BeginFade(1, speed);
        yield return new WaitForSeconds(speed);
        Quit();
    }

    /// <summary>
    /// appelé naturellement quand on ferme le jeu
    /// </summary>
    private void OnApplicationQuit()
    {
        if (closing)
            return;
        Quit();
    }

    /// <summary>
    /// quite le jeu (si on est dans l'éditeur, quite le mode play)
    /// </summary>
    [ContextMenu("Quit")]
    private void Quit()
    {
        closing = true;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
    #endregion
}