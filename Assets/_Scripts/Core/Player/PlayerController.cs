using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// PlayerController handle player movement
/// <summary>
public class PlayerController : MonoBehaviour, IKillable
{
    #region Attributes
    [SerializeField]
    private float baseJumpForce = 5.0F;

    [SerializeField]
    private float lowJumpMultiplier = 1.025F;


    [FoldoutGroup("Gameplay"), Tooltip("Mouvement du joueur"), SerializeField]
    private float moveSpeed = 10.0F;

    [FoldoutGroup("Gameplay"), Tooltip("MaxMove"), SerializeField]
    private float maxMoveSpeed = 20.0F;

    [FoldoutGroup("Debug"), Tooltip("MaxMove"), SerializeField]
    private int idPlayer = 0;


    // Components
    private Rigidbody playerBody;
    private FrequencyTimer updateTimer;
	private float horizMove;
    private float vertiMove;
    private bool hasMoved = false;
    private bool isJumping = false;

    #endregion

    #region Initialize

    private void Awake()
	{
		playerBody = GetComponent<Rigidbody> ();
	}

    #endregion

    #region Core
    private void InputPlayer()
    {
        horizMove = PlayerConnected.GetSingleton.getPlayer(idPlayer).GetAxis("Move Horizontal");
        vertiMove = PlayerConnected.GetSingleton.getPlayer(idPlayer).GetAxis("Move Vertical");
        isJumping = PlayerConnected.GetSingleton.getPlayer(idPlayer).GetButtonDown("FireA");

        if (horizMove != 0 || vertiMove != 0)
            hasMoved = true;
        else
            hasMoved = false;
    }

    private void MovePlayer()
    {
        if (hasMoved)
        {
            playerBody.velocity = new Vector3(horizMove * moveSpeed, playerBody.velocity.y, 0.0F);
        }
        if (isJumping)
        {
            SoundManager.GetSingleton.playSound("Jump");
            isJumping = false;
        }
    }
    
    /////////////////////////////////////////////////////

	private void Update()
	{
        InputPlayer();
    }

	private void FixedUpdate()
	{
        MovePlayer();
	}

    #endregion

    [FoldoutGroup("Debug"), Button("Kill")]
    public void Kill()
	{
		Debug.Log ("Dead");
        Destroy(gameObject);
	}
}