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
    private List<GameObject> prefabsPowers1List;
    [FoldoutGroup("GamePlay"), Tooltip("balls prefabs"), SerializeField]
    public List<GameObject> prefabsPowers2List;

    public int GetSizePowerList(int powerType)
    {
        if (powerType == 0)
            return (prefabsPowers1List.Count);
        else
            return (prefabsPowers2List.Count);
    }
    public GameObject GetPowerPrefabs(int powerType, int id)
    {
        if (powerType == 0)
            return (prefabsPowers1List[id]);
        else
            return (prefabsPowers2List[id]);
    }


    [FoldoutGroup("GamePlay"), Tooltip("color players"), SerializeField]
    private List<Color> colorPlayer;
    public List<Color> ColorPlayer { get { return (colorPlayer); } }

    [FoldoutGroup("GamePlay"), Tooltip("balls prefabs"), SerializeField]
    private List<Material> materialPlayer;
    public List<Material> MaterialPlayer { get { return (materialPlayer); } }

    [FoldoutGroup("Scenes"), Tooltip("liens du levelManager"), SerializeField]
    private MenuManager menuManager;
    public MenuManager MenuManagerScript { set { menuManager = value; InitMenulWhenLoaded(); } }

    [FoldoutGroup("Scenes"), Tooltip("liens du levelManager"), SerializeField]
    private SetupManager setupManager;
    public SetupManager SetupManagerScript { set { setupManager = value; InitSetuplWhenLoaded(); } }

    [FoldoutGroup("Scenes"), Tooltip("liens du levelManager"), SerializeField]
    private LevelManager levelManager;
    public LevelManager LevelManager { set { levelManager = value; InitLevelWhenLoaded(); } }

    
    [FoldoutGroup("Debug"), Tooltip("opti fps"), SerializeField]
	private FrequencyTimer updateTimer;

    [FoldoutGroup("Debug"), Tooltip("opti fps"), SerializeField]
    private PlayerBallInit playerBallInit;
    public PlayerBallInit PlayerBallInit { get { return playerBallInit; } }


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

    /// <summary>
    /// est appelé quand le menu est chargé !
    /// </summary>
    private void InitMenulWhenLoaded()
    {
        Debug.Log("menu ready");
    }
    /// <summary>
    /// appelé quand les joypad se co/deco.
    /// </summary>
    public void CallChangePhase()
    {
        playerBallInit.SetupGamePadActive();
        if (setupManager)
            setupManager.ChangePhase();
    }

    /// <summary>
    /// est appelé quand le menu est chargé !
    /// </summary>
    private void InitSetuplWhenLoaded()
    {
        Debug.Log("setup ready");
        setupManager.InitSetup();
    }

    /// <summary>
    /// appelé lorsque le level viens de se charger
    /// </summary>
    private void InitLevelWhenLoaded()
    {
        ScoreManager.GetSingleton.ResetAll();   //reset les scores
        playerBallInit.Setup();                 //SETUP les données de créations de joueurs (si besoin !);

        levelManager.StartGame();               //commence le jeu (spawn etc);
    }
    /// <summary>
    /// appelé lorsque, depuis le game, on restart la scene courante
    /// restart: si c'est true, alors on a restart, sinon, on fait juste un start...
    /// </summary>
    public void RestartGame(bool restart)
    {
        playerBallInit.FromLevel = true;
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
            id = GetSizePowerList(type) - 1;


        if (id >= GetSizePowerList(type))
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

        return (GetPowerPrefabs(id, type));

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
