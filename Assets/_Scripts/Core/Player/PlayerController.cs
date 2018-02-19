using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

[Serializable]
public struct PlayerBall
{
    [Tooltip("le type de ball (bleu, red...), selon la liste de prefabs du gameManager")]
    public int idBallType;             //le type de ball (bleu, red...)
    [Tooltip("les types de pouvoirs de la balle (x2 pouvoir), selon la liste de prefabs du gameManager")]
    public int[] powers;    //pouvoirs de la balle gauche

    PlayerBall(int idBall, int[] powersBall)
    {
        idBallType = idBall;
        powers = powersBall;
    }
}

/// <summary>
/// PlayerController handle player movement
/// <summary>
public class PlayerController : MonoBehaviour, IKillable
{
    #region Attributes

    [FoldoutGroup("GamePlay"), Tooltip("vitesse de rotation des followers (de base, à multiplier par le ratio des balls"), SerializeField]
    private float turnRateFollowers = 400f;

    [FoldoutGroup("GamePlay"), Tooltip("les infos des 2 balls à créé sur le player + ses weapons"), SerializeField]
    private PlayerBall[] ballInfo = new PlayerBall[SizeArrayId];
    public PlayerBall[] BallInfo { get { return ballInfo; } }

    [FoldoutGroup("Objects"), Tooltip("List des followers qui suivent les balls"), SerializeField]
    private List<Transform> followersList;
    public List<Transform> FollowersList { get { return followersList; } }

    [FoldoutGroup("Objects"), Tooltip("objet vide contenant les balls"), SerializeField]
    private Transform parentBalls;
    public Transform ParentBalls { get { return parentBalls; } }


    [FoldoutGroup("Objects"), Tooltip("rope reliant les 2 followers"), SerializeField]
    private GameObject rope;
    public Transform Rope { get { return rope.transform; } }


    [FoldoutGroup("Debug"), Tooltip("id unique du joueur correspondant à sa manette"), SerializeField]
    private int idPlayer = 0;
    public int IdPlayer { get { return idPlayer; } }


    [FoldoutGroup("Debug"), Tooltip("List des balls créé"), SerializeField]
    private List<Balls> ballsList;
    public List<Balls> BallsList { get { return ballsList; } }

    [FoldoutGroup("Debug"), Tooltip("playerController"), SerializeField]
    private Rope ropeScript;
    public Rope RopeScript { get { return ropeScript; } }

    private FrequencyTimer updateTimer;
    private const int SizeArrayId = 2;  //nombre de ball du joueur

    #endregion

    #region Initialize

    private void Awake()
	{
        InitPlayer();
	}

    /// <summary>
    /// initialise les players: créé les balls et les ajoutes dans la liste si la liste est vide
    /// </summary>
    private void InitPlayer()
    {
        //ici reset la liste si il n'y en a pas 2
        if (ballsList.Count != 2)
            ClearListBall();    //debug si il n'y a pas 2 emplacements vide pour les balls
        ChangeBalls();
        //Invoke("initRope", 1.0f);
        initRope();
    }

    private void ClearListBall()
    {
        Debug.Log("Clear la liste des balls");
        for (int i = 0; i < ballsList.Count; i++)
        {
            if (ballsList[i])
                ballsList[i].Kill();
        }
        ballsList.Clear();
        ballsList.Add(null);
        ballsList.Add(null);
    }

    private void initRope()
    {
        ropeScript.InitPhysicRope();
    }

    /// <summary>
    /// Change la ball du joueur:
    /// idBall: 0 ou 1 (gauche ou droite ?)
    /// idTypeBall: type de ball
    /// </summary>
    /// <param name="idBall"></param>
    /// <param name="idTypeBall"></param>
    [Button("ChangeBalls")]
    private void ChangeBalls()
    {
        for (int i = 0; i < ballsList.Count; i++)
        {
            //SI il y a déja une ball... et que le type voulu est le même, ne pas changer de ball...
            if (ballsList[i] && ballsList[i].IdBall == ballInfo[i].idBallType)
            {
                //ici ne pas recréé la ball, on veut peut etre changer les pouvoirs par contre ??
                ballsList[i].InitBall(this, i); //ici init la ball avec les pouvoirs
            }
            //ici il y a déjà une ball, MAIS on veut une ball différente...
            else if (ballsList[i] && ballsList[i].IdBall != ballInfo[i].idBallType)
            {
                //on supprime l'existante pour créé la nouvelle
                ballsList[i].Kill();

                CreateBall(i);
            }
            //sinon, si il y a rien dans la liste, créé la ball voulu tout simplement !
            else if (!ballsList[i])
            {
                CreateBall(i);
            }
        }
    }
    private void CreateBall(int index)
    {
        GameObject ballsObject = Instantiate(GameManager.GetSingleton.GiveMeBall(ballInfo[index].idBallType), followersList[index].position, followersList[index].rotation, parentBalls);
        ballsList[index] = ballsObject.GetComponent<Balls>();
        ballsList[index].InitBall(this, index); //ici init la ball avec les pouvoirs
    }

    #endregion

    #region Core
    /// <summary>
    /// test si la ball passé en parametre est contenue dans le player
    /// </summary>
    public bool isContainingThisBall(GameObject ball)
    {
        if (!ballsList[0] || !ballsList[1])
            return (false);

        if (ball.GetInstanceID() == ballsList[0].gameObject.GetInstanceID()
            || ball.GetInstanceID() == ballsList[1].gameObject.GetInstanceID())
        {
            return (true);
        }
        return (false);
    }

    /// <summary>
    /// test si le link passé en paramettre est contenue dans la rope du player
    /// </summary>
    /// <param name="link">Link est l'objet link à tester</param>
    public bool isContainingThisLink(GameObject link)
    {
        Line rope = link.transform.parent.GetComponent<Line>();
        if (!rope)
            return (false);

        if (rope.PlayerControllerVariable == this)
        {
            return (true);
        }
        return (false);
    }

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
            if (!ballsList[i])
            {
                if (followersList[i].gameObject.activeInHierarchy)
                {
                    Debug.Log("la balle n'existe pas....");
                    followersList[i].gameObject.SetActive(false);
                }
                continue;
            }

            Vector3 pos = ballsList[i].gameObject.transform.position;
            followersList[i].position = pos;    //set position of balls

            if (ballsList[i].HasMoved)  //set rotation
                followersList[i].rotation = QuaternionExt.DirObject(followersList[i].rotation, ballsList[i].HorizMove, -ballsList[i].VertiMove, turnRateFollowers * ballsList[i].RatioTurnRateFocus);
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

        if (!followersList[0].gameObject.activeInHierarchy
            && !followersList[1].gameObject.activeInHierarchy)
        {
            Kill();
        }
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
		Debug.Log ("Player dead !");
        Destroy(gameObject);
	}
}