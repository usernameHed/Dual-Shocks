using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

/// <summary>
/// PlayerController handle player movement
/// <summary>
public class PlayerController : MonoBehaviour, IKillable
{
    #region Attributes

    [FoldoutGroup("GamePlay"), Tooltip("vitesse de rotation des followers"), SerializeField]
    private float turnRateFollowers = 400f;


    [FoldoutGroup("Objects"), Tooltip("List des followers qui suivent les balls"), SerializeField]
    private List<Transform> followersList;

    [FoldoutGroup("Objects"), Tooltip("objet vide contenant les balls"), SerializeField]
    private Transform parentBalls;
    [FoldoutGroup("Objects"), Tooltip("List des balls qui suivent les balls"), SerializeField]
    private List<Balls> ballsList;

    [FoldoutGroup("Objects"), Tooltip("rope reliant les 2 followers"), SerializeField]
    private GameObject rope;

    [FoldoutGroup("Objects"), Tooltip("balls prefabs"), SerializeField]
    private GameObject balls;

    [FoldoutGroup("Debug"), Tooltip("id unique du joueur correspondant à sa manette"), SerializeField]
    private int idPlayer = 0;
    public int IdPlayer { get { return idPlayer; } }

    // Components
    private FrequencyTimer updateTimer;

    #endregion

    #region Initialize

    private void Awake()
	{
        InitPlayer();
        InitBall();
	}

    private void Start()
    {
        InitRope();
    }

    /// <summary>
    /// initialise les players: créé les balls et les ajoutes dans la liste si la liste est vide
    /// </summary>
    private void InitPlayer()
    {
        if (ballsList.Count < 2)
            ballsList.Add(null);
        if (!ballsList[0])
        {
            GameObject ballsObject = Instantiate(balls, followersList[0].position, followersList[0].rotation, parentBalls);
            ballsList[0] = ballsObject.GetComponent<Balls>();
        }
        if (!ballsList[1])
        {
            GameObject ballsObject = Instantiate(balls, followersList[1].position, followersList[1].rotation, parentBalls);
            ballsList[1] = ballsObject.GetComponent<Balls>();
        }
    }

    /// <summary>
    /// initialise les balls (les id)
    /// </summary>
    private void InitBall()
    {
        for (int i = 0; i < ballsList.Count; i++)
        {
            ballsList[i].InitBall(this, i);
        }
    }

    /// <summary>
    /// initialisation de la rope
    /// </summary>
    private void InitRope()
    {
        //list: ballsList
    }

    #endregion

    #region Core
    /// <summary>
    /// input of player for both joystick
    /// </summary>
    private void InputPlayer()
    {
        for (int i = 0; i < ballsList.Count; i++)
        {
            ballsList[i].HorizMove = PlayerConnected.GetSingleton.getPlayer(idPlayer).GetAxis("Move Horizontal" + ((i == 0) ? "" : " Right") );
            ballsList[i].VertiMove = PlayerConnected.GetSingleton.getPlayer(idPlayer).GetAxis("Move Vertical" + ((i == 0) ? "" : " Right"));

            ballsList[i].Power1 = PlayerConnected.GetSingleton.getPlayer(idPlayer).GetButtonDown( ((i == 0) ? "Left" : "Right") + "Trigger1");
            ballsList[i].Power2 = PlayerConnected.GetSingleton.getPlayer(idPlayer).GetAxis( ((i == 0) ? "Left" : "Right") + "Trigger2");

            if (ballsList[i].HorizMove != 0 || ballsList[i].VertiMove != 0)
                ballsList[i].HasMoved = true;
            else
                ballsList[i].HasMoved = false;
        }
    }

    /// <summary>
    /// change position of follower according to balls
    /// - first: input player on update
    /// - then: balls move on fixedUpdate (physics)
    /// - then: after physcis calculation, change position followers
    /// </summary>
    private void ChangePosFollower()
    {
        for (int i = 0; i < followersList.Count; i++)
        {
            Vector3 pos = ballsList[i].gameObject.transform.position;
            followersList[i].position = pos;    //set position of balls

            if (ballsList[i].HasMoved)  //set rotation
                followersList[i].rotation = QuaternionExt.DirObject(followersList[i].rotation, ballsList[i].HorizMove, -ballsList[i].VertiMove, turnRateFollowers);
        }
    }

    #endregion

    #region Unity ending functions
    /// <summary>
    /// input du joueur
    /// </summary>
    private void Update()
	{
        InputPlayer();

    }

	private void FixedUpdate()
	{
        
	}

    /// <summary>
    /// après les mouvements physique, set la position des followers
    /// </summary>
    private void LateUpdate()
    {
        ChangePosFollower();
    }

    #endregion

    [FoldoutGroup("Debug"), Button("Kill")]
    public void Kill()
	{
		Debug.Log ("Dead");
        Destroy(gameObject);
	}
}