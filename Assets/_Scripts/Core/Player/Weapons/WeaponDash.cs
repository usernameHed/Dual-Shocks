using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Networking;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class WeaponDash : Weapon
{
    #region public variable
    /// <summary>
    /// variable public
    /// </summary>
    [FoldoutGroup("Gameplay")] [Tooltip("la vitesse de turnRate du weapon")] [SerializeField] private float turnRate = 2000f;
    [FoldoutGroup("Gameplay")] [Tooltip("Impulsion du joueur lors du tir")] [SerializeField] private float forceImpulse = 10f;
    [FoldoutGroup("Gameplay")] [Tooltip("cree la rocket un peu devant ?")] [SerializeField] private float forwardPoint = 0f;

    /// <summary>
    /// variable public HideInInspector
    /// </summary>
    //[HideInInspector] public bool tmp;

    #endregion

    #region private variable
    /// <summary>
    /// variable privé
    /// </summary>
	private Rigidbody playerBody;
    private float timeToGo;

    /// <summary>
    /// variable privé serealized
    /// </summary>
    [FoldoutGroup("Debug")] [Tooltip("Optimisation des fps")] [SerializeField] [Range(0, 10.0f)] private float timeOpti = 0.1f;
    [FoldoutGroup("Debug")] [Tooltip("rocket prefabs")] [SerializeField] private GameObject prefabsRocket;

    #endregion
    
    #region  initialisation
    /// <summary>
    /// Initialisation à l'activation
    /// </summary>
    private void Start()
    {
        timeToGo = Time.fixedTime + timeOpti;                               //setup le temps
		//playerBody = PC.GetComponent<Rigidbody>();
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
        //SoundManager.Instance.PlaySound (SoundManager.Instance.RocketLaunchSound);

		Debug.Log("Dash");
		
		//playerBody.AddForce(transform.up * -forceImpulse, ForceMode.Impulse);
    }
    #endregion

    #region unity fonction and ending

    /// <summary>
    /// effectué à chaque frame
    /// </summary>
    private void Update()
    {
        //effectué à chaque opti frame
        if (Time.fixedTime >= timeToGo)
        {
            //ici action optimisé

            timeToGo = Time.fixedTime + timeOpti;
        }
    }

   
    #endregion
}
