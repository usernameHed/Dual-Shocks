using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

[Serializable]
public struct PlayerBall
{
    public int idBallType;             //le type de ball (bleu, red...)
    public int[] powerJoystickLeft;    //pouvoirs de la balle gauche
    public int[] powerJoystickRight;   //pouvoirs de la balle droite

    PlayerBall(int idBall, int[] powerLeft, int[] powerRight)
    {
        idBallType = idBall;
        powerJoystickLeft = powerLeft;
        powerJoystickRight = powerRight;
    }
}

/// <summary>
/// PlayerController handle player movement
/// <summary>
public class PlayerController : MonoBehaviour, IKillable
{
    #region Attributes

    [FoldoutGroup("GamePlay"), Tooltip("vitesse de rotation des followers"), SerializeField]
    private float turnRateFollowers = 400f;

    [FoldoutGroup("GamePlay"), Tooltip("l'id du type de ball à créé (gauche et droite)"), SerializeField]
    private PlayerBall[] ballInfo = new PlayerBall[SizeArrayId];


    [FoldoutGroup("Objects"), Tooltip("List des followers qui suivent les balls"), SerializeField]
    private List<Transform> followersList;

    [FoldoutGroup("Objects"), Tooltip("objet vide contenant les balls"), SerializeField]
    private Transform parentBalls;
    [FoldoutGroup("Objects"), Tooltip("List des balls qui suivent les balls"), SerializeField]
    private List<Balls> ballsList;

    [FoldoutGroup("Objects"), Tooltip("rope reliant les 2 followers"), SerializeField]
    private GameObject rope;


    [FoldoutGroup("Rope"), Tooltip("Position de l'anchor pour les 2 balles"), SerializeField]
    private Vector3 ropeAnchors = Vector3.zero;

    [FoldoutGroup("Rope"), Tooltip("Force de l'élastique"), SerializeField]
    private float spring = 10;

    [FoldoutGroup("Rope"), Tooltip("Force de l'amortissement"), SerializeField]
    private float damper = 0.2f;
    

    [FoldoutGroup("Debug"), Tooltip("id unique du joueur correspondant à sa manette"), SerializeField]
    private int idPlayer = 0;
    public int IdPlayer { get { return idPlayer; } }

    private FrequencyTimer updateTimer;
    private const int SizeArrayId = 2;  //nombre de ball du joueur

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
        //ici reset la liste si il n'y en a pas 2
        if (ballsList.Count != 2)
        {
            Debug.Log("erreur !!");
            ballsList.Clear();
            ballsList.Add(null);
            ballsList.Add(null);
        }
        ChangeBall();

        /*
        //setup la ball 1 (si il n'y en a pas déjà une !)
        if (!ballsList[0])
        {
            GameObject ballsObject = Instantiate(prefabsBallsList[0], followersList[0].position, followersList[0].rotation, parentBalls);
            ballsList[0] = ballsObject.GetComponent<Balls>();
        }
        //setup la ball 2 (si il n'y en a pas déjà une !)
        if (!ballsList[1])
        {
            GameObject ballsObject = Instantiate(prefabsBallsList[0], followersList[1].position, followersList[1].rotation, parentBalls);
            ballsList[1] = ballsObject.GetComponent<Balls>();
        }
        */
    }

    /// <summary>
    /// Change la ball du joueur:
    /// idBall: 0 ou 1 (gauche ou droite ?)
    /// idTypeBall: type de ball
    /// </summary>
    /// <param name="idBall"></param>
    /// <param name="idTypeBall"></param>
    [Button("ChangeBall")]
    private void ChangeBall()
    {
        for (int i = 0; i < ballsList.Count; i++)
        {
            GameObject ballsObject = Instantiate(GameManager.GetSingleton.GiveMeBall(ballInfo[i].idBallType), followersList[i].position, followersList[i].rotation, parentBalls);
            ballsList[i] = ballsObject.GetComponent<Balls>();
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
        //Add Spring joint component and initialise its values
        SpringJoint joint = ballsList[0].gameObject.AddComponent<SpringJoint>();
        joint.connectedBody = ballsList[1].gameObject.GetComponent<Rigidbody>();
        joint.anchor = ropeAnchors;
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = ropeAnchors;
        joint.spring = spring;
        joint.damper = damper;
        joint.enableCollision = true;
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

    /// <summary>
    /// fonction de debug pour éviter de resizer le tableau d'id
    /// </summary>
    void OnValidate()
    {
        if (ballInfo.Length != SizeArrayId)
        {
            Debug.LogWarning("Don't change the 'ints' field's array size!");
            Array.Resize(ref ballInfo, SizeArrayId);
        }
    }

    [FoldoutGroup("Debug"), Button("Kill")]
    public void Kill()
	{
		Debug.Log ("Dead");
        Destroy(gameObject);
	}
}