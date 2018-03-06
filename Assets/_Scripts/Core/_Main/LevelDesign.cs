﻿using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// LevelDesign Description
/// </summary>
public class LevelDesign : MonoBehaviour
{
    //protected LevelDesign() { } // guarantee this will be always a singleton only - can't use the constructor!

    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("idLevel"), SerializeField]
    private int idLevel = 0;
    [FoldoutGroup("GamePlay"), Tooltip("idLevel"), SerializeField]
    private GameObject contentLevel;

    private static LevelDesign instance;
    public static LevelDesign GetSingleton
    {
        get { return instance; }
    }
    #endregion

    #region Initialization

    /// <summary>
    /// singleton
    /// </summary>
    public void SetSingleton()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Awake()
    {
        /*if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
            return;
        }*/

        SetSingleton();
    }
    #endregion

    #region Core
    public void InitLevelDesign()
    {
        //s'il est déja actif... peut être le réinitialiser ??
        //ou rechanger les données ???
        if (contentLevel.activeSelf)
        {
            Debug.Log("Error ici !!");
            //contentLevel.SetActive(false);
            //contentLevel.SetActive(true);
        }
        else
        {
            contentLevel.SetActive(true);
        }
    }

    public void DesactiveScene()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Unity ending functions

	#endregion
}
