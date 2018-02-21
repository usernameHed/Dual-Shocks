using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// ScoreManager Description
/// </summary>
public class ScoreManager : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("Debug"), Tooltip("Sauvegarde du joueur"), SerializeField]
    private PlayerData data = new PlayerData();
    public PlayerData Data { get { return data; } }


    private static ScoreManager instance;
    public static ScoreManager GetSingleton
    {
        get { return instance; }
    }
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

    private void Awake()
    {
        SetSingleton();
    }

    /// <summary>
    /// lors du start: load les données, sinon, sauvegarder !
    /// </summary>
    private void Start()
    {
        if (!Load())
            ResetAll();
    }
    #endregion

    #region Core

    /// <summary>
    /// 
    /// </summary>
    public void ResetAll()
    {
        if (data == null)
            data = new PlayerData();
        data.SetDefault();

        Save();
    }


    /// <summary>
    /// renvoi VRAI si ça à loadé !
    /// </summary>
    /// <returns></returns>
    [FoldoutGroup("Debug"), Button("load")]
    public bool Load()
    {
        data = DataSaver.Load<PlayerData>("playerData.dat");
        if (data == null)
            return (false);
        return (!(data == null));
    }


    [FoldoutGroup("Debug"), Button("save")]
    public void Save()
    {
        DataSaver.Save(data);
    }

    [FoldoutGroup("Debug"), Button("delete")]
    public void Delete()
    {
        DataSaver.DeleteSave("playerData.dat");
    }
    #endregion

    #region Unity ending functions

    #endregion
}
