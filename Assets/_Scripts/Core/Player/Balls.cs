using UnityEngine;
using Sirenix.OdinInspector;
using System.Collections.Generic;

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
    [FoldoutGroup("GamePlay"), Tooltip("Ratio du turnRate de la visée"), SerializeField]
    private float ratioTurnRateFocus = 1;
    public float RatioTurnRateFocus { get { return ratioTurnRateFocus; } }

    [FoldoutGroup("Debug"), Tooltip("List des weapons de la ball créé"), SerializeField]
    private List<Weapon> weaponsList;
    public List<Weapon> WeaponsList { get { return weaponsList; } }

    [FoldoutGroup("Debug"), Tooltip("opti fps"), SerializeField]
	private FrequencyTimer updateTimer;

    private Rigidbody ballBody;
    public Rigidbody BallBody { get { return ballBody; } }

    public float HorizMove { set; get; }
    public float VertiMove { set; get; }
    public bool HasMoved { set; get; }
    private bool power1;
    public bool Power1
    {
        set
        {
            if (value != power1 && value == false && weaponsList[0])
            {
                weaponsList[0].OnShootRelease();
            }
            power1 = value;
        }
        get
        {
            return power1;
        }
    }
    private float power2;
    public float Power2
    {
        set
        {
            if (value != power2 && value == 0 && weaponsList[1])
            {
                weaponsList[1].OnShootRelease();
            }
            power2 = value;
        }
        get
        {
            return power2;
        }
    }

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

        InitWeapon();
    }

    /// <summary>
    /// cree les 2 weapons du player
    /// </summary>
    private void InitWeapon()
    {
        
        if (weaponsList.Count != 2)
            ClearListWeapons();    //debug si il n'y a pas 2 emplacements vide pour les balls
        ChangeWeapons();

        
    }
    #endregion

    #region Core
    [Button("ChangeWeapons")]
    private void ChangeWeapons()
    {
        Debug.Log("ici créé les 2 weapons !!");
        for (int i = 0; i < weaponsList.Count; i++)
        {
            //SI il y a déja une weapon... et que le type voulu est le même, ne pas changer de weapon...
            if (weaponsList[i] && weaponsList[i].IdWeapon == playerRef.BallInfo[idBallPlayer].powers[i])
            {
                //peut etre ré-init le pouvoir ??
                weaponsList[i].InitWeapon(playerRef, this, idBallPlayer); //ici init le pouvoir
            }
            //ici il y a déjà une weapon, MAIS on veut une weapon différente...
            else if (weaponsList[i] && weaponsList[i].IdWeapon != playerRef.BallInfo[idBallPlayer].powers[i])
            {
                weaponsList[i].Kill();
                CreateWeapon(i);
            }
            //sinon, si il y a rien dans la liste, créé la ball voulu tout simplement !
            else if (!weaponsList[i])
            {
                CreateWeapon(i);
            }
        }


        activated = true;   //active la ball
    }
    private void CreateWeapon(int index)
    {
        int idWeapon = playerRef.BallInfo[idBallPlayer].powers[index];
        if (!isAllowedToCreateWeapon(idWeapon, index))
        {
            Debug.Log("ici pas de weapon n°" + (index + 1));
            return;
        }

        GameObject weaponObject = Instantiate(GameManager.GetSingleton.GiveMePower(idWeapon), transform.position, playerRef.FollowersList[idBallPlayer].rotation, transform);

        weaponsList[index] = weaponObject.GetComponent<Weapon>();
        weaponsList[index].InitWeapon(playerRef, this, idBallPlayer); //ici init la ball avec les pouvoirs
    }

    /// <summary>
    /// défini si le weapon peut être créé à partir de son id (idWeaponTOCreate)
    /// </summary>
    private bool isAllowedToCreateWeapon(int idWeaponToCreate, int indexWeapon)
    {
        //l'id est incorrect
        if (idWeaponToCreate < 0 || idWeaponToCreate >= GameManager.GetSingleton.PrefabsPowerCount())
            return (false);

        //l'id du 2eme weapon est le même que le premier... ne rien faire !
        if (indexWeapon == 1 && idWeaponToCreate == weaponsList[0].IdWeapon)
            return (false);
        return (true);
    }

    private void ClearListWeapons()
    {
        Debug.Log("Clear la liste des balls");
        for (int i = 0; i < weaponsList.Count; i++)
        {
            if (weaponsList[i])
                weaponsList[i].Kill();
        }
        weaponsList.Clear();
        weaponsList.Add(null);
        weaponsList.Add(null);
    }

    private void MovePlayer()
    {
        if (HasMoved)
        {
            ballBody.AddForce(HorizMove * moveSpeed * Time.deltaTime, 0.0f, VertiMove * moveSpeed * Time.deltaTime, ForceMode.Impulse);
        }

        if (Power1 && weaponsList[0])
        {
            weaponsList[0].TryShoot();
            //Debug.Log("power 1 of [playerId " + playerRef.IdPlayer + ", ballId: " + IdBallPlayer + "] activated");
        }            
        if (Power2 > 0 && weaponsList[1])
        {
            weaponsList[1].TryShoot();
            //Debug.Log("power 2 of [playerId " + playerRef.IdPlayer + ", ballId: " + IdBallPlayer + "] activated with " + Power2);
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
        if (weaponsList[0])
            weaponsList[0].Kill();
        if (weaponsList[1])
            weaponsList[1].Kill();
        Destroy(gameObject);
    }
}
