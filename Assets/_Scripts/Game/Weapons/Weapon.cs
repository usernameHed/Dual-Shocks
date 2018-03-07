using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Weapon : MonoBehaviour, IKillable
{
    #region variable
    //variable gameplay for all weapons
    [FoldoutGroup("Gameplay"), Tooltip("est-ce qu'on reste appuyé ou pas ?"), SerializeField]
    private bool hold = false;

    [FoldoutGroup("Gameplay"), DisableIf("hold"), Tooltip("cooldown de l'arme (pour les attaque unique)"), SerializeField]
    private float cooldown = 0;

    //objects
    [FoldoutGroup("Object"), Tooltip("ref sur l'animator"), SerializeField]
    protected GameObject display;

    [FoldoutGroup("Debug"), Tooltip("ref sur playerController"), SerializeField]
    protected PlayerController playerRef;
    [FoldoutGroup("Debug"), Tooltip("ref sur la ball"), SerializeField]
    protected Balls ballRef;
    [FoldoutGroup("Debug"), Tooltip("id de l'arme"), SerializeField]
    private float idWeapon;
    public float IdWeapon { get { return idWeapon; } }
    [FoldoutGroup("Debug"), Tooltip("est-ce un cooldown ?"), SerializeField]
    private bool weaponCoolDown;
    public bool WeaponCoolDown { get { return weaponCoolDown; } }
    [FoldoutGroup("Debug"), Tooltip("id (0 ou 1) du numéro du weapoin par rapport au joueur"), SerializeField]
    protected int orderWeapon;
    public int OrderWeapon { get { return orderWeapon; } }
    [FoldoutGroup("Debug"), Tooltip("opti fps"), SerializeField]
    protected FrequencyTimer updateTimer;

    protected bool activatedScript = true;

    private float nextShoot;
    private bool holding = false;

    #endregion

    #region  initialisation
    /// <summary>
    /// fonctiion appelé pour tout les weapons
    /// </summary>
    public void InitWeapon(PlayerController refPlayer, Balls refBall, int orderOfWeapon)
    {
        activatedScript = true;

        playerRef = refPlayer;
        ballRef = refBall;
        orderWeapon = orderOfWeapon;

        display.transform.SetParent(playerRef.FollowersList[orderWeapon].transform);
        display.transform.position = playerRef.FollowersList[orderWeapon].transform.position;

        InitParticularWeapon();    //initialise le weapon particulier
    }

    /// <summary>
    /// Initialisation à l'activation
    /// </summary>
    private void Start()
    {
        holding = false;
    }
    #endregion

    #region  core script
    
	public void TryShoot()
	{
        //si on est en mode cooldown
		if (nextShoot <= 0 && !hold)
		{
            if (!OnShoot())
                return;
    		nextShoot = cooldown;
		}

        //si on est en mode hold...
        else if (nextShoot <= 0 && hold)    //ici un potentiel cooldown si on a plus de thrust...
        {
            //si c'est la première fois qu'on hold... on appelle la fonction shoot
            if (!holding)
            {
                OnShoot();
                holding = true;
            }
            else //sinon, on est en holding...
            {
                HoldShoot();
            }
        }
	}

    private void HoldShoot()
    {
        //isShooting = true;
        OnShootHold();
    }

    /// <summary>
    /// est appelé lorsque le joueur lache la touche
    /// </summary>
    public void ReleaseShoot()
    {
        if (!hold)  //n'as pas de sens si on est dans un weapon a usage unique
            return;
        holding = false;
        nextShoot = cooldown;
        OnShootRelease();
    }

    //appelé obligatoirement dans les fils
    abstract protected void InitParticularWeapon(); //appelé à l'initialisation
    abstract protected bool OnShoot ();             //appelé lors du shoot

    //appelé SI besoin (si on fait du hold)
    protected virtual void OnShootHold() { }          //appelé lorsque l'on reste appuyé
    protected virtual void OnShootRelease() { }          //appelé lorsque l'on reste appuyé


    public virtual float WeaponPercent()
	{
		return Mathf.Clamp((cooldown - nextShoot) / cooldown, 0.0F, 1.0f);
	}

    #endregion

    #region unity ending

    /// <summary>
    /// exécuté après update
    /// </summary>
    private void LateUpdate()
    {
        if (nextShoot > 0)
        {
            nextShoot -= Time.deltaTime;
        }
    }
    #endregion

    [FoldoutGroup("Debug")]
    [Button("Kill")]
    public void Kill()
    {
        if (!activatedScript)
            return;

        activatedScript = false;
        display.transform.SetParent(transform); //remet le display avant de la supprimer !
        //SoundManager.Instance.PlaySound (SoundManager.Instance.RocketSound);
        Destroy(gameObject);
    }
}
