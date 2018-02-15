using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class WeaponStop : Weapon
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    [FoldoutGroup("Gameplay"), Tooltip("drag à mettre à la ball quand on stop"), SerializeField]
    private float dragWhenStop = 3f;

    [FoldoutGroup("Gameplay"), Tooltip("Max Fuel (sec)"), SerializeField]
    private float maxFuel = 3f;

	[FoldoutGroup("Gameplay"), Tooltip("Fuel gain multiplier"), SerializeField]
    private float fuelIncreaseMultiplier = 1.0f;

    [FoldoutGroup("Object"), Tooltip("l'objet à mettre actif lors du pouvoir"), SerializeField]
    private GameObject Spiks;

    private float saveDrag = 0;

    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>
    private float timeToGo;
    private Rigidbody playerBody;
    private bool isShooting = false;
	private float fuel;
	private bool fireHeld;
	private bool malus;
    /// <summary>
    /// variable privé serealized
    /// </summary>
    [FoldoutGroup("Debug")]
    [Tooltip("Optimisation des fps")] [SerializeField] [Range(0, 10.0f)] private float timeOpti = 0.1f;

    #endregion

    #region  initialisation
    /// <summary>
    /// Initialisation à l'activation
    /// </summary>
    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;
        //playerBody = PC.GetComponent<Rigidbody>();
        //particle.GetComponent<TriggerFlame>().setActive(PC.gameObject);
		fuel = maxFuel;
        //saveCoolDown = cooldown;
        //cooldown = 0;
        saveDrag = ballRef.BallBody.drag;
    }

    /// <summary>
    /// appelé lors de l'initialisation de ce weapon
    /// </summary>
    protected override void InitParticularWeapon()
    {
        Debug.Log("init this weapon id: " + IdWeapon);
    }
    #endregion

    #region core script
    /// <summary>
    /// functionTest
    /// </summary>
	protected override void Shoot()
    {
		if ((malus && fuel < maxFuel) || fuel <= 0.0F)
		{
			//PC.jetpack = false;
			return;
		}
        if (!fireHeld)
        {
            Debug.Log("here first Throwing");
            Spiks.SetActive(true);
            //cooldown = 0;
        }
        else
        {
            Debug.Log("Throwing...");
            ballRef.BallBody.drag = dragWhenStop;
            //cooldown = 0;
        }
		//PC.jetpack = true;
		fireHeld = true;
        
		isShooting = true;
        timeToGo = Time.fixedTime + timeOpti;                               //setup le temps

    }

    public override void OnShootRelease()
    {
        fireHeld = false;
        Spiks.SetActive(false);
        ballRef.BallBody.drag = saveDrag;

        Debug.Log("no more throw...");
        //cooldown = saveCoolDown;
        //timeToGo = Time.fixedTime + timeOpti;
        //SoundManager.Instance.PlaySound (SoundManager.Instance.EmptyFlameThrowerSound);
    }


    private void RpcAddForceToPlayer(Rigidbody Rbplayer, Vector3 impulsion)
    {
        if (Rbplayer)
            Rbplayer.AddForce(impulsion * Time.deltaTime, ForceMode.Acceleration);
    }

    #endregion

    #region unity fonction and ending

    /// <summary>
    /// effectué à chaque frame
    /// </summary>
    private void Update()
    {
        //effectué à chaque opti frame
        if (isShooting && Time.fixedTime >= timeToGo)
        {
            isShooting = false;
            //particle.SetActive(false);
            
            OnShootRelease();

            //particle.GetComponent<ParticleSystem>().Stop();
            //ici action optimisé

            //timeToGo = Time.fixedTime + timeOpti;
        }


		// Is weapon shooting ?
		if (fireHeld)
		{
			//SoundManager.Instance.PlaySound (SoundManager.Instance.FlameThrowerSound);
			fuel = Mathf.Max (fuel - Time.deltaTime, 0.0F);
			malus = fuel == 0.0F;
		}
		else
		{
			fuel = Mathf.Min (fuel + Time.deltaTime * fuelIncreaseMultiplier, maxFuel);
			malus = fuel == maxFuel;
		}
    }

	public override float WeaponPercent()
	{
		return Mathf.Clamp(fuel / maxFuel, 0.0F, 1.0F);
	}

    
    #endregion
}
