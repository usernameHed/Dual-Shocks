using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Balls Description
/// </summary>
public class Balls : MonoBehaviour, IKillable
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("speed of balls"), SerializeField]
    private float moveSpeed = 200f;
    [FoldoutGroup("GamePlay"), Tooltip("l'id qui défini le type de ball"), SerializeField]
    private int idBall = 0;
    public int IdBall { get { return idBall; } }

    [FoldoutGroup("Debug"), Tooltip("opti fps"), SerializeField]
	private FrequencyTimer updateTimer;

    private Rigidbody ballBody;
    public Rigidbody BallBody { get { return ballBody; } }

    public float HorizMove { set; get; }
    public float VertiMove { set; get; }
    public bool HasMoved { set; get; }
    public bool Power1 { set; get; }
    public float Power2 { set; get; }

    private int idBallPlayer = -1;
    public int IdBallPlayer { get { return idBallPlayer; } }   //l'id (0 ou 1) de la balle par rapport au joueur (balle de gauche ou droite ?)
    private bool activated = false; //la ball est-elle activé ?
    private PlayerController playerRef;

    #endregion

    #region Initialization

    private void Awake()
    {
        //InitBall();
    }

    /// <summary>
    /// initialise la ball
    /// </summary>
    public void InitBall(PlayerController player, int id)
    {
        HasMoved = false;
        Power1 = false;
        Power2 = 0f;
        
        ballBody = gameObject.GetComponent<Rigidbody>();

        playerRef = player;
        idBallPlayer = id;

        CreateWeapon();

        
    }
    #endregion

    #region Core
    /// <summary>
    /// cree les 2 weapons du player
    /// </summary>
    private void CreateWeapon()
    {
        Debug.Log("ici créé les 2 weapons !!");

        activated = true;   //active la ball
    }

    private void MovePlayer()
    {
        if (HasMoved)
        {
            ballBody.AddForce(HorizMove * moveSpeed * Time.deltaTime, 0.0f, VertiMove * moveSpeed * Time.deltaTime, ForceMode.Impulse);
        }

        if (Power1)
            Debug.Log("power 1 of [playerId " + playerRef.IdPlayer + ", ballId: " + IdBallPlayer + "] activated");
        if (Power2 > 0)
            Debug.Log("power 2 of [playerId " + playerRef.IdPlayer + ", ballId: " + IdBallPlayer + "] activated with " + Power2);
        /*if (isJumping)
        {
            SoundManager.GetSingleton.playSound("Jump");
            isJumping = false;
        }*/
    }
    #endregion

    #region Unity ending functions

    private void Update()
    {
        if (!activated) //si la ball n'est pas activé, ne rien faire
            return;

        //optimisation des fps
        if (updateTimer.Ready())
        {

        }
    }

    private void FixedUpdate()
    {
        if (!activated) //si la ball n'est pas activé, ne rien faire
            return;
        MovePlayer();
    }

    #endregion

    [FoldoutGroup("Debug"), Button("Kill")]
    public void Kill()
    {
        Debug.Log("Dead");
        Destroy(gameObject);
    }
}
