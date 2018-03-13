using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

/// <summary>
/// PlayerController handle player movement
/// <summary>
public class PlayerController : MonoBehaviour, IKillable
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("Attend X seconde avant d'activer les inputs"), SerializeField]
    private float waitBeforeActive = 1f;

    [FoldoutGroup("GamePlay"), Tooltip("vitesse de rotation des followers (de base, à multiplier par le ratio des balls"), SerializeField]
    private float turnRateFollowers = 400f;

    [FoldoutGroup("GamePlay"), Tooltip("les infos des 2 balls à créé sur le player + ses weapons"), SerializeField]
    private PlayerBall[] ballInfo = new PlayerBall[SizeArrayId];
    public PlayerBall[] BallInfo { set { ballInfo = value; } get { return ballInfo; } }

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
    public int IdPlayer { set { idPlayer = value; } get { return idPlayer; } }


    [FoldoutGroup("Debug"), Tooltip("List des balls créé"), SerializeField]
    private List<Balls> ballsList;
    public List<Balls> BallsList { get { return ballsList; } }

    [FoldoutGroup("Debug"), Tooltip("List des balls créé"), SerializeField]
    private Transform[] spawnBall = new Transform[2];
    public Transform[] SpawnBall { get { return spawnBall; } }
    

    [FoldoutGroup("Debug"), Tooltip("playerController"), SerializeField]
    private Rope ropeScript;
    public Rope RopeScript { get { return ropeScript; } }

    private FrequencyTimer updateTimer;
    private const int SizeArrayId = 2;  //nombre de ball du joueur

    private int ballRemaining = SizeArrayId;
    private bool enabledPlayer = true;
    private bool stopAction = false;

    #endregion

    #region Initialize

    private void OnEnable()
	{
        EventManager.StartListening(GameData.Event.GameOver, StopAction);
        //EventManager.StartListening(GameData.Event.GameOver, StopAction);
        InitPlayer();
	}

    /// <summary>
    /// initialise les players: créé les balls et les ajoutes dans la liste si la liste est vide
    /// </summary>
    private void InitPlayer()
    {
        Debug.Log("init player: " + idPlayer);
        enabledPlayer = true;
        //stopAction = false;
        ropeScript.gameObject.SetActive(true);
        ActiveFollower(true);

        //ici reset la liste si il n'y en a pas 2
        if (ballsList.Count != 2)
            ClearListBall();    //debug si il n'y a pas 2 emplacements vide pour les balls
        ChangeBalls();

        ballRemaining = SizeArrayId;
        
        InitRope();
        
        stopAction = true;
        Invoke("StartAction", waitBeforeActive);
    }

    /// <summary>
    /// est appelé pour init les position des spawn au début
    /// </summary>
    public void SpawnBallPos(Transform pos1, Transform pos2)
    {
        spawnBall[0] = pos1;
        spawnBall[1] = pos2;
        followersList[0].position = spawnBall[0].position;
        followersList[1].position = spawnBall[1].position;
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

    private void InitRope()
    {
        ropeScript.InitObjects(ballsList[0].gameObject, ballsList[1].gameObject, Rope);
        ropeScript.InitPhysicRope();
        ropeScript.ChangeColorLink(GameManager.GetSingleton.ColorPlayer[idPlayer]);
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
        GameObject ballsObject = Instantiate(GameManager.GetSingleton.GiveMeBall(ballInfo[index].idBallType), spawnBall[index].position, Quaternion.identity/*followersList[index].rotation*/, parentBalls);


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
    /// input of player for both joystick
    /// </summary>
    private void InputPlayer()
    {
        for (int i = 0; i < ballsList.Count; i++)
        {
            if (!ballsList[i])
                continue;
            ballsList[i].HorizMove = PlayerConnected.Instance.getPlayer(idPlayer).GetAxis("Move Horizontal" + ((i == 0) ? "" : " Right") );
            ballsList[i].VertiMove = PlayerConnected.Instance.getPlayer(idPlayer).GetAxis("Move Vertical" + ((i == 0) ? "" : " Right"));

            ballsList[i].Power1 = PlayerConnected.Instance.getPlayer(idPlayer).GetButtonDown( ((i == 0) ? "Left" : "Right") + "Trigger1");
            ballsList[i].Power2 = PlayerConnected.Instance.getPlayer(idPlayer).GetAxis( ((i == 0) ? "Left" : "Right") + "Trigger2");

            if (ballsList[i].HorizMove != 0 || ballsList[i].VertiMove != 0)
                ballsList[i].HasMoved = true;
            else
                ballsList[i].HasMoved = false;
        }
    }

    /// <summary>
    /// Dès qu'on a bougé, enlever la kinematie des 2 balls
    /// </summary>
    public void UnsetKinematic()
    {
        if (ballsList[0])
            ballsList[0].UnsetKinematic();
        if (ballsList[1])
            ballsList[1].UnsetKinematic();
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
            if (!ballsList[i] || !followersList[i].gameObject.activeSelf)
            {
                continue;
            }

            Vector3 pos = ballsList[i].gameObject.transform.position;
            followersList[i].position = pos;    //set position of balls


            if (ballsList[i].HasMoved)  //set rotation
            {
                followersList[i].rotation = QuaternionExt.DirObject(followersList[i].rotation, ballsList[i].HorizMove, -ballsList[i].VertiMove, turnRateFollowers * ballsList[i].RatioTurnRateFocus);
            }

        }
    }

    /// <summary>
    /// active les bon followers
    /// </summary>
    private void ActiveFollower(bool active)
    {
        followersList[0].gameObject.SetActive(active);
        followersList[1].gameObject.SetActive(active);
    }

    /// <summary>
    /// est appelé pour voir si la dernière ball est en train d'être détruite...
    /// position de l'explosion...
    /// </summary>
    public void TestForDestroyLink(Vector3 position, int indexBall)
    {
        Debug.Log("ici plusieurs fois ??");
        ballRemaining--;
        //s'il ne reste qu'une ball qui se fait actuellement détruire...
        if (ballRemaining <= 0)
        {
            //rope.transform.SetParent(null);
            //ropeScript.Kill();
            Debug.Log("ici call kill & break");
            Invoke("Kill", ropeScript.TimeToBecomeHarmLess + (ropeScript.RationRandom * (ropeScript.LinkCircular.Count + 1)));
            ropeScript.JustBreakUpLink(position);
        }
        else
        {
            //ici il y a une ball encore fonctionnelle
            ballsList[(indexBall == 0) ? 1 : 0].IAmTheOnlyOne(true);
            ropeScript.OnlyOneMainLeft(true);
        }
    }

    private void StartAction()
    {
        stopAction = false;
        Debug.Log("ici le joueur peut jouer !");
    }
    /// <summary>
    /// stop les action du player...
    /// </summary>
    private void StopAction()
    {
        stopAction = true;
        for (int i = 0; i < ballsList.Count; i++)
        {
            if (!ballsList[i])
                continue;
            ballsList[i].HorizMove = 0;
            ballsList[i].VertiMove = 0;

            ballsList[i].Power1 = false;
            ballsList[i].Power2 = 0;

            ballsList[i].HasMoved = false;
        }
        Debug.Log("stop action du joueur");
    }

    #endregion

    #region Unity ending functions
    /// <summary>
    /// input du joueur
    /// </summary>
    private void Update()
	{
        if (!stopAction)
            InputPlayer();
    }

    /// <summary>
    /// après les mouvements physique, set la position des followers
    /// </summary>
    private void LateUpdate()
    {
        ChangePosFollower();
    }

    private void OnDisable()
    {
        EventManager.StopListening(GameData.Event.GameOver, StopAction);
    }

    #endregion

    [FoldoutGroup("Debug"), Button("Kill")]
    public void Kill()
	{
        if (!enabledPlayer)
            return;
        RopeScript.Kill();

		Debug.Log ("Player dead !");
        enabledPlayer = false;
        

        Debug.Log("ici envoi du trigger...");
        EventManager.TriggerEvent(GameData.Event.PlayerDeath, idPlayer);
        gameObject.SetActive(false);
    }
}