using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;
using Sirenix.Serialization;

/// <summary>
/// GameManager Description
/// </summary>
public class GameManager : SerializedMonoBehaviour
{
    #region Attributes

    [FoldoutGroup("GamePlay"), Tooltip("balls prefabs"), SerializeField]
    private List<GameObject> prefabsBallsList;

    [FoldoutGroup("GamePlay"), Tooltip("balls prefabs"), NonSerialized, OdinSerialize]
    private List<List<GameObject>> prefabsPowersList;
    public GameObject PrefabsPowersList(int x, int y) { return (prefabsPowersList[x][y]); }
    public int PrefabsPowerListCount(int x) { return (prefabsPowersList[x].Count); }

    [FoldoutGroup("GamePlay"), Tooltip("color players"), SerializeField]
    private List<Color> colorPlayer;
    public List<Color> ColorPlayer { get { return (colorPlayer); } }

    [FoldoutGroup("GamePlay"), Tooltip("balls prefabs"), SerializeField]
    private List<Material> materialPlayer;
    public List<Material> MaterialPlayer { get { return (materialPlayer); } }


    //////////////////////////////////////////////////////////////////////
    [FoldoutGroup("Scenes"), Tooltip("liens du levelManager"), SerializeField]
    private SceneManagerLocal sceneManagerLocal;
    public SceneManagerLocal SceneManagerLocal { set { sceneManagerLocal = value; InitNewScene(); } get { return (sceneManagerLocal); } }

    [FoldoutGroup("Debug"), Tooltip("opti fps"), SerializeField]
	private FrequencyTimer updateTimer;

    [FoldoutGroup("Debug"), Tooltip("opti fps"), SerializeField]
    private PlayerBallInit playerBallInit;
    public PlayerBallInit PlayerBallInit { get { return playerBallInit; } }

    private bool fromGame = false;
    public bool FromGame { get { return (fromGame); } set { fromGame = value; } }

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

    private void OnEnable()
    {
        EventManager.StartListening(GameData.Event.GamePadConnectionChange, CallChangePhase);
    }

    private void Awake()
    {
        SetSingleton();
    }

    /// <summary>
    /// initialise les ILevelManagers (il y en a forcément 1 par niveau)
    /// </summary>
    private void InitNewScene()
    {
        if (sceneManagerLocal.LevelManagerScript != null)
            sceneManagerLocal.LevelManagerScript.InitScene();
    }

    /// <summary>
    /// appelé quand les joypad se co/deco.
    /// </summary>
    public void CallChangePhase(bool active, int id)
    {
        playerBallInit.SetupGamePadActive();

        if (sceneManagerLocal.LevelManagerScript != null)
            sceneManagerLocal.LevelManagerScript.CallGamePad();
    }

    /// <summary>
    /// appelé lorsque, depuis le game, on restart la scene courante
    /// restart: si c'est true, alors on a restart, sinon, on fait juste un start...
    /// </summary>
    public void RestartGame(bool restart)
    {
        playerBallInit.FromLevel = true;
    }

    /// <summary>
    /// est appelé depuis le LevelManager de game quand on restart...
    /// </summary>
    /// <param name="isFromGame"></param>
    public void FromGameToSetup(bool isFromGame)
    {
        fromGame = isFromGame;
    }
    #endregion

    #region Core
    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">l'id actuel de la ball</param>
    /// <param name="loop">est-ce qu'on loop ou on clamp ?</param>
    /// <returns></returns>
    public int GiveMeGoodIdBall(int id, bool loop)
    {
        if (loop)
        {
            if (id < 0)
                id = prefabsBallsList.Count - 1;
            if (id >= prefabsBallsList.Count)
                id = 0;
        }
        else
        {
            if (id < 0)
                id = 0;
            if (id >= prefabsBallsList.Count)
                id = prefabsBallsList.Count - 1;
        }
        return (id);
    }
    /// <summary>
    /// selon l'id, renvoi le bon prefabs de ball à créé
    /// </summary>
    /// <returns></returns>
    public GameObject GiveMeBall(int id)
    {
        id = GiveMeGoodIdBall(id, true);

        return (prefabsBallsList[id]);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id">l'id actuel de la ball</param>
    /// <param name="loop">est-ce qu'on loop ou on clamp ?</param>
    /// <returns></returns>
    public int GiveMeGoodIdPower(int id, int type)
    {

        if (id < 0)
            id = PrefabsPowerListCount(type) - 1;


        if (id >= PrefabsPowerListCount(type))
            id = 0;

        return (id);
    }
    /// <summary>
    /// selon l'id, renvoi le bon prefabs du pouvoir
    /// </summary>
    /// <returns></returns>
    public GameObject GiveMePower(int id, int type)
    {

        id = GiveMeGoodIdPower(id, type);

        return (PrefabsPowersList(type, id));

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

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GamePadConnectionChange, CallChangePhase);
    }
    #endregion
}
