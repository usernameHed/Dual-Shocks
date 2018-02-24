using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System;

/// <summary>
/// PlayerController handle player movement
/// <summary>
public class PlayerControllerMenu : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("GamePlay"), Tooltip("clamp distance"), SerializeField]
    private float clampDistance = 5f;
    [FoldoutGroup("GamePlay"), Tooltip("speed"), SerializeField]
    private float speed = 2f;
    [FoldoutGroup("GamePlay"), Tooltip("speed to get back to normal"), SerializeField]
    private float speedBack = 0.5f;


    [FoldoutGroup("Debug"), Tooltip("id unique du joueur correspondant à sa manette"), SerializeField]
    private int idPlayer = 0;
    public int IdPlayer { set { idPlayer = value; } get { return idPlayer; } }


    [FoldoutGroup("Debug"), Tooltip("List des balls créé"), SerializeField]
    private List<GameObject> ballsList;
    public List<GameObject> BallsList { get { return ballsList; } }

    [FoldoutGroup("Debug"), Tooltip("playerController"), SerializeField]
    private Rope ropeScript;
    public Rope RopeScript { get { return ropeScript; } }

    [FoldoutGroup("Objects"), Tooltip("rope reliant les 2 followers"), SerializeField]
    private GameObject rope;
    public Transform Rope { get { return rope.transform; } }

    private float[] horizMove = new float[2];
    private float[] vertiMove = new float[2];
    private bool[] hasMoved = new bool[2];
    private bool[] power1 = new bool[2];
    private float[] power2 = new float[2];
    private Vector3[] initialPos = new Vector3[2];

    #endregion

    #region Initialize

    private void OnEnable()
	{
        InitPlayer();
	}

    /// <summary>
    /// initialise les players: créé les balls et les ajoutes dans la liste si la liste est vide
    /// </summary>
    private void InitPlayer()
    {
        ChangeBalls();

        initialPos[0] = ballsList[0].transform.position;
        initialPos[1] = ballsList[1].transform.position;

        initRope();
    }

    private void initRope()
    {
        ropeScript.InitObjects(ballsList[0].gameObject, ballsList[1].gameObject, Rope);
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
        /*for (int i = 0; i < ballsList.Count; i++)
        {
            //SI il y a déja une ball... et que le type voulu est le même, ne pas changer de ball...
            if (ballsList[i] && ballsList[i].IdBall == ballInfo[i].idBallType)
            {
                //ici ne pas recréé la ball

            }
            //ici il y a déjà une ball, MAIS on veut une ball différente...
            else if (ballsList[i] && ballsList[i].IdBall != ballInfo[i].idBallType)
            {
                //on supprime l'existante pour créé la nouvelle
                Destroy(ballsList[i]);
                CreateBall(i);
            }
            //sinon, si il y a rien dans la liste, créé la ball voulu tout simplement !
            else if (!ballsList[i])
            {
                CreateBall(i);
            }
        }*/
    }
    private void CreateBall(int index)
    {
        /*GameObject ballsObject = Instantiate(GameManager.GetSingleton.GiveMeBall(ballInfo[index].idBallType), followersList[index].position, followersList[index].rotation, parentBalls);
        Destroy(ballsObject.GetComponent<Balls>());*/
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
            horizMove[i] = PlayerConnected.GetSingleton.getPlayer(idPlayer).GetAxis("Move Horizontal" + ((i == 0) ? "" : " Right") );
            vertiMove[i] = PlayerConnected.GetSingleton.getPlayer(idPlayer).GetAxis("Move Vertical" + ((i == 0) ? "" : " Right"));

            power1[i] = PlayerConnected.GetSingleton.getPlayer(idPlayer).GetButtonDown( ((i == 0) ? "Left" : "Right") + "Trigger1");
            power2[i] = PlayerConnected.GetSingleton.getPlayer(idPlayer).GetAxis( ((i == 0) ? "Left" : "Right") + "Trigger2");

            if (horizMove[i] != 0 || vertiMove[i] != 0)
                hasMoved[i] = true;
            else
                hasMoved[i] = false;
        }
    }

    private void MovePlayer()
    {
        for (int i = 0; i < ballsList.Count; i++)
        {
            Rigidbody rb = ballsList[i].GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            ballsList[i].transform.position = Vector3.MoveTowards(ballsList[i].transform.position, new Vector3(initialPos[i].x, initialPos[i].y, 0), speedBack * Time.deltaTime);
            

            if (hasMoved[i])
            {
                


                //ballsList[i].GetComponent<Rigidbody>().MovePosition(pos);
                //rb.MovePosition(new Vector3(horizMove[i] * speed * Time.deltaTime, 0.0f, vertiMove[i] * speed * Time.deltaTime));
                //rb.AddForce(horizMove[i] * speed * Time.deltaTime, 0.0f, vertiMove[i] * speed * Time.deltaTime, ForceMode.Impulse);
                ballsList[i].transform.Translate(new Vector3(horizMove[i] * speed * Time.deltaTime, vertiMove[i] * speed * Time.deltaTime, 0), Space.World);
                Vector3 pos = ballsList[i].transform.position;
                pos.x = Mathf.Clamp(pos.x, initialPos[i].x - clampDistance, initialPos[i].x + clampDistance);
                pos.y = Mathf.Clamp(pos.y, initialPos[i].y - clampDistance, initialPos[i].y + clampDistance);
                //pos.z = Mathf.Clamp(pos.z, initialPos[i].z - clampDistance, initialPos[i].z + clampDistance);
                ballsList[i].transform.position = pos;
            }

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
        MovePlayer();
	}

    #endregion
}