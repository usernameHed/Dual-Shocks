using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Weapon : MonoBehaviour, IKillable
{
    [FoldoutGroup("Gameplay"), Tooltip("cooldown de l'arme"), SerializeField]
    private float cooldown;

    

    [FoldoutGroup("Object"), Tooltip("ref sur l'animator"), SerializeField]
    protected GameObject display;
    [FoldoutGroup("Object"), Tooltip("ref sur l'animator"), SerializeField]
    protected Animator animator;

    [FoldoutGroup("Debug"), Tooltip("ref sur playerController"), SerializeField]
    protected PlayerController playerRef;
    [FoldoutGroup("Debug"), Tooltip("ref sur la ball"), SerializeField]
    protected Balls ballRef;
    [FoldoutGroup("Debug"), Tooltip("cooldown de l'arme"), SerializeField]
    private float idWeapon;
    public float IdWeapon { get { return idWeapon; } }
    [FoldoutGroup("Debug"), Tooltip("id (0 ou 1) du numéro du weapoin par rapport au joueur"), SerializeField]
    protected int orderWeapon;
    public int OrderWeapon { get { return orderWeapon; } }


    private float nextShoot;

	void LateUpdate()
	{
		if (nextShoot > 0)
		{
			nextShoot -= Time.deltaTime;
		}
	}

	public void TryShoot()
	{
		if (nextShoot <= 0)
		{
			nextShoot = cooldown;
			Shoot ();
		}
	}

    /// <summary>
    /// fonctiion appelé pour tout les weapons
    /// </summary>
    public void InitWeapon(PlayerController refPlayer, Balls refBall, int orderOfWeapon)
    {
        playerRef = refPlayer;
        ballRef = refBall;
        orderWeapon = orderOfWeapon;

        display.transform.SetParent(playerRef.FollowersList[orderWeapon].transform);

        InitParticularWeapon();    //initialise le weapon particulier
    }

    abstract protected void InitParticularWeapon();
    abstract protected void Shoot ();

    public virtual void OnShootRelease(){}

	public virtual float WeaponPercent()
	{
		return Mathf.Clamp((cooldown - nextShoot) / cooldown, 0.0F, 1.0F);
	}

    [FoldoutGroup("Debug")]
    [Button("Kill")]
    public void Kill()
    {
        display.transform.SetParent(transform); //remet le display avant de la supprimer !
        //SoundManager.Instance.PlaySound (SoundManager.Instance.RocketSound);
        Destroy(gameObject);
    }
}
