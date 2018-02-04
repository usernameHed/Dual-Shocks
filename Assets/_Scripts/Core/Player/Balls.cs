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
    public bool Power2 { set; get; }

    #endregion

    #region Initialization

    private void Awake()
    {
        InitBall();
        Debug.Log("awake ball");
    }

    /// <summary>
    /// initialise la ball
    /// </summary>
    private void InitBall()
    {
        HasMoved = false;
        Power1 = false;
        Power2 = false;
        ballBody = gameObject.GetComponent<Rigidbody>();
    }
    #endregion

    #region Core
    private void MovePlayer()
    {
        if (HasMoved)
        {
            //ballBody.velocity = new Vector3(HorizMove * moveSpeed * Time.deltaTime, 0.0f, VertiMove * moveSpeed * Time.deltaTime);
            ballBody.AddForce(HorizMove * moveSpeed * Time.deltaTime, 0.0f, VertiMove * moveSpeed * Time.deltaTime, ForceMode.Impulse);
        }

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
        //optimisation des fps
        if (updateTimer.Ready())
        {

        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    #endregion
}
