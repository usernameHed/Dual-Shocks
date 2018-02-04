using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

/// <summary>
/// PlayerController handle player movement
/// <summary>
public class PlayerController : MonoBehaviour, IKillable
{
    #region Attributes
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

    

    // Components
    private FrequencyTimer updateTimer;

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

    #endregion

    #region Core
    private void InputPlayer()
    {
        for (int i = 0; i < ballsList.Count; i++)
        {
            ballsList[i].HorizMove = PlayerConnected.GetSingleton.getPlayer(idPlayer).GetAxis("Move Horizontal" + ((i == 0) ? "" : " Right") );
            ballsList[i].VertiMove = PlayerConnected.GetSingleton.getPlayer(idPlayer).GetAxis("Move Vertical" + ((i == 0) ? "" : " Right"));

            ballsList[i].Power1 = PlayerConnected.GetSingleton.getPlayer(idPlayer).GetButtonDown("LeftTrigger" + (i + 1));
            ballsList[i].Power2 = PlayerConnected.GetSingleton.getPlayer(idPlayer).GetButtonDown("RightTrigger" + (i + 2));

            if (ballsList[i].HorizMove != 0 || ballsList[i].VertiMove != 0)
                ballsList[i].HasMoved = true;
            else
                ballsList[i].HasMoved = false;
        }
    }

    private void MovePlayer()
    {
        /*if (hasMoved)
        {
            playerBody.velocity = new Vector3(horizMove * moveSpeed, playerBody.velocity.y, 0.0F);
        }*/

        /*if (isJumping)
        {
            SoundManager.GetSingleton.playSound("Jump");
            isJumping = false;
        }*/
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