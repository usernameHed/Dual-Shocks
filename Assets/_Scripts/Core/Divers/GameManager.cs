using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;


/// <summary>
/// GameManager Description
/// </summary>
public class GameManager : MonoBehaviour
{
    #region Attributes

    [FoldoutGroup("GamePlay"), Tooltip("balls prefabs"), SerializeField]
    private List<GameObject> prefabsBallsList;

    [FoldoutGroup("GamePlay"), Tooltip("balls prefabs"), SerializeField]
    private List<GameObject> prefabsPowersList;
    public int PrefabsPowerCount() { return (prefabsPowersList.Count); }

    [FoldoutGroup("Debug"), Tooltip("liens du levelManager"), SerializeField]
    private LevelManager levelManager;
    public LevelManager LevelManager { set { levelManager = value; InitLevelWhenLoaded(); } }

    [FoldoutGroup("Debug"), Tooltip("opti fps"), SerializeField]
	private FrequencyTimer updateTimer;

    

    private static GameManager instance;
    public static GameManager GetSingleton
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
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Awake()
    {
        SetSingleton();
    }
    #endregion

    #region Core
    /// <summary>
    /// appelé lorsque le level viens de se charger
    /// </summary>
    private void InitLevelWhenLoaded()
    {
        ScoreManager.GetSingleton.ResetAll();   //reset les scores
        levelManager.StartGame();               //commence le jeu (spawn etc);
    }

    /// <summary>
    /// selon l'id, renvoi le bon prefabs de ball à créé
    /// </summary>
    /// <returns></returns>
    public GameObject GiveMeBall(int id)
    {
        if (id < 0 || id >= prefabsBallsList.Count)
        {
            Debug.Log("error id ball");
            Debug.Break();
            return (null);
        }
        return (prefabsBallsList[id]);
    }

    /// <summary>
    /// selon l'id, renvoi le bon prefabs du pouvoir
    /// </summary>
    /// <returns></returns>
    public GameObject GiveMePower(int id)
    {
        if (id < 0 || id >= prefabsPowersList.Count)
        {
            Debug.Log("error id power");
            Debug.Break();
            return (null);
        }
        return (prefabsPowersList[id]);
    }
    #endregion

    #region Unity ending functions

    private void Update()
    {
        //optimisation des fps
        if (updateTimer.Ready())
        {

        }
    }

	#endregion
}
