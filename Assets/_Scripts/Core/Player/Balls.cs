using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Balls Description
/// </summary>
public class Balls : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("speed of balls"), SerializeField]
    private float moveSpeed = 200f;


    [FoldoutGroup("Debug"), Tooltip("opti fps"), SerializeField]
	private FrequencyTimer updateTimer;

    private Rigidbody ballBody;

    public float HorizMove { set; get; }
    public float VertiMove { set; get; }
    public bool HasMoved { set; get; }
    public bool Power1 { set; get; }
    public float Power2 { set; get; }

    private int idBall = -1;
    public int IdBall { get { return idBall; } }   //l'id (0 ou 1) de la balle par rapport au joueur
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
        idBall = id;

        activated = true;
    }
    #endregion

    #region Core
    private void MovePlayer()
    {
        if (HasMoved)
        {
            ballBody.AddForce(HorizMove * moveSpeed * Time.deltaTime, 0.0f, VertiMove * moveSpeed * Time.deltaTime, ForceMode.Impulse);
        }

        if (Power1)
            Debug.Log("power 1 of [playerId " + playerRef.IdPlayer + ", ballId: " + IdBall + "] activated");
        if (Power2 > 0)
            Debug.Log("power 2 of [playerId " + playerRef.IdPlayer + ", ballId: " + IdBall + "] activated with " + Power2);
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
}
