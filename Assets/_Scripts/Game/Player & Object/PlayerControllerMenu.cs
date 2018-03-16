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
    [FoldoutGroup("GamePlay"), Tooltip("Clamp la speed (debug pour gerer les masse...)"), SerializeField]
    private float minSpeedToDivised = 10f;
    [FoldoutGroup("GamePlay"), Tooltip("marge de positionnement pour changer de ball"), SerializeField]
    private float margeChangeBall = 0.1f;
    [FoldoutGroup("GamePlay"), Tooltip("marge de positionnement pour changer de ball"), SerializeField]
    private bool loopBallAndPower = true;

    [FoldoutGroup("Objects"), Tooltip("rope reliant les 2 followers"), SerializeField]
    private GameObject rope;
    public Transform Rope { get { return rope.transform; } }

    [FoldoutGroup("Debug"), Tooltip("ref du SetupManager"), SerializeField]
    private SetupManager setupManager;

    [FoldoutGroup("Debug"), Tooltip("active ou pas le joueur"), SerializeField]
    private bool enabledScript = true;
    public bool EnabledScript { set { enabledScript = value; } get { return enabledScript; } }


    [FoldoutGroup("Debug"), Tooltip("id unique du joueur correspondant à sa manette"), SerializeField]
    private int idPlayer = 0;
    public int IdPlayer { set { idPlayer = value; } get { return idPlayer; } }


    [FoldoutGroup("Debug"), Tooltip("List des balls créé"), SerializeField]
    private List<GameObject> ballsList;
    public List<GameObject> BallsList { get { return ballsList; } }

    [FoldoutGroup("Debug"), Tooltip("playerController"), SerializeField]
    private Rope ropeScript;
    public Rope RopeScript { get { return ropeScript; } }

    [FoldoutGroup("Debug"), Tooltip("coolDown balls"), SerializeField]
    private FrequencyTimer[] coolDownBalls = new FrequencyTimer[2];
    [FoldoutGroup("Debug"), Tooltip("coolDown power 1"), SerializeField]
    private FrequencyTimer[] coolDownPowers1 = new FrequencyTimer[2];
    [FoldoutGroup("Debug"), Tooltip("coolDown power 2"), SerializeField]
    private FrequencyTimer[] coolDownPowers2 = new FrequencyTimer[2];


    private float[] horizMove = new float[2];
    private float[] vertiMove = new float[2];
    private bool[] hasMoved = new bool[2];
    private bool[] power1 = new bool[2];
    private float[] power2 = new float[2];
    private Vector3[] initialPos = new Vector3[2];
    private float[] massBall = new float[2];
    private float[] sizeScale = new float[2];

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
        if (!enabledScript)
            return;
        Debug.Log("changement de ball !");
        DataPlayers dataPlayer = GameManager.Instance.PlayerBallInit.PlayerData[idPlayer];

        for (int i = 0; i < ballsList.Count; i++)
        {
            int idBall = dataPlayer.ballInfo[i].idBallType;
            GameObject ball = GameManager.Instance.GiveMeBall(idBall);

            massBall[i] = ball.GetComponent<Rigidbody>().mass;
            sizeScale[i] = ball.transform.localScale.x;

            ballsList[i].transform.localScale = new Vector3(sizeScale[i], sizeScale[i], sizeScale[i]);
        }
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
            horizMove[i] = PlayerConnected.Instance.getPlayer(idPlayer).GetAxis("Move Horizontal" + ((i == 0) ? "" : " Right") );
            vertiMove[i] = PlayerConnected.Instance.getPlayer(idPlayer).GetAxis("Move Vertical" + ((i == 0) ? "" : " Right"));

            power1[i] = PlayerConnected.Instance.getPlayer(idPlayer).GetButtonDown( ((i == 0) ? "Left" : "Right") + "Trigger1");
            power2[i] = PlayerConnected.Instance.getPlayer(idPlayer).GetAxis( ((i == 0) ? "Left" : "Right") + "Trigger2");

            if (horizMove[i] != 0 || vertiMove[i] != 0)
                hasMoved[i] = true;
            else
                hasMoved[i] = false;
        }
    }

    /// <summary>
    /// ici éffectue les changements de ball / pouvoir
    /// </summary>
    private void ChangeFromInput()
    {
        for (int i = 0; i < ballsList.Count; i++)
        {
            //Changement de ball
            Vector3 pos = ballsList[i].transform.position;
            if (pos.x - margeChangeBall <= initialPos[i].x - clampDistance && coolDownBalls[i].Ready())
            {
                int id = GameManager.Instance.PlayerBallInit.PlayerData[idPlayer].ballInfo[i].idBallType;
                id = GameManager.Instance.GiveMeGoodIdBall(id - 1, loopBallAndPower);
                GameManager.Instance.PlayerBallInit.PlayerData[idPlayer].ballInfo[i].idBallType = id;
                ChangeBalls();
            }
            if (pos.x + margeChangeBall >= initialPos[i].x + clampDistance && coolDownBalls[i].Ready())
            {
                int id = GameManager.Instance.PlayerBallInit.PlayerData[idPlayer].ballInfo[i].idBallType;
                id = GameManager.Instance.GiveMeGoodIdBall(id + 1, loopBallAndPower);
                GameManager.Instance.PlayerBallInit.PlayerData[idPlayer].ballInfo[i].idBallType = id;
                ChangeBalls();
            }

            //ici power 1 activé
            if (power1[i] && coolDownPowers1[i].Ready())
            {
                int id = GameManager.Instance.PlayerBallInit.PlayerData[idPlayer].ballInfo[i].powers[0];
                id = GameManager.Instance.GiveMeGoodIdPower(id + 1, 0);
                GameManager.Instance.PlayerBallInit.PlayerData[idPlayer].ballInfo[i].powers[0] = id;
                setupManager.DisplayInSetupScript.ChangePowerDisplay();
            }
            //ici power 2 activé
            if (power2[i] >= 1.0f && coolDownPowers2[i].Ready())
            {
                int id = GameManager.Instance.PlayerBallInit.PlayerData[idPlayer].ballInfo[i].powers[1];
                id = GameManager.Instance.GiveMeGoodIdPower(id + 1, 1);
                GameManager.Instance.PlayerBallInit.PlayerData[idPlayer].ballInfo[i].powers[1] = id;
                setupManager.DisplayInSetupScript.ChangePowerDisplay();
            }
        }
    }

    private void MovePlayer()
    {
        for (int i = 0; i < ballsList.Count; i++)
        {
            Rigidbody rb = ballsList[i].GetComponent<Rigidbody>();
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;




            float speedBackModified = speedBack / Mathf.Min(massBall[i], minSpeedToDivised);

            ballsList[i].transform.position = Vector3.MoveTowards(ballsList[i].transform.position, new Vector3(initialPos[i].x, initialPos[i].y, 0), speedBackModified * Time.deltaTime);
            

            if (hasMoved[i])
            {

                float speedModified = speed / Mathf.Min(massBall[i], minSpeedToDivised);

                ballsList[i].transform.Translate(new Vector3(horizMove[i] * speedModified * Time.deltaTime, vertiMove[i] * speedModified * Time.deltaTime, 0), Space.World);
                Vector3 pos = ballsList[i].transform.position;
                pos.x = Mathf.Clamp(pos.x, initialPos[i].x - clampDistance, initialPos[i].x + clampDistance);
                pos.y = Mathf.Clamp(pos.y, initialPos[i].y - clampDistance, initialPos[i].y + clampDistance);
                ballsList[i].transform.SetXY(pos.x, pos.y);
                //ballsList[i].transform.position = pos;
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
        if (!enabledScript)
            return;
        InputPlayer();
        ChangeFromInput();
    }

	private void FixedUpdate()
	{
        if (!enabledScript)
            return;
        MovePlayer();
	}

    #endregion
}