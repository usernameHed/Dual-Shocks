using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Networking;

/// <summary>
/// description
/// <summary>

[RequireComponent(typeof(ShockWaveEffect))]
public class WeaponShockWave : Weapon
{
    #region variable
    /// <summary>
    /// variable
    /// </summary>
    [FoldoutGroup("Gameplay"), Tooltip("Impulsion du joueur lors du tir"), SerializeField]
    private float forceImpulse = 10f;
    [FoldoutGroup("Gameplay"), Tooltip("radius de l'explosion"), SerializeField]
    private float radius = 5f;
    [FoldoutGroup("Gameplay"), Tooltip("Nom des layers à chercher et à pousser"), SerializeField]
    private string[] layerToPush;

    private ShockWaveEffect shockwaveEffect;

    #endregion

    #region  initialisation
    private void Awake()
    {
        shockwaveEffect = GetComponent<ShockWaveEffect>();
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
	protected override void OnShoot()
    {
        Debug.Log("shockwave !");

        //ballRef.BallBody.AddForce(Vector3.up * forceImpulse, ForceMode.VelocityChange);
        shockwaveEffect.CreateWave(ballRef.transform);
        SoundManager.GetSingleton.playSound("ShockWave" + transform.GetInstanceID().ToString());
        CreateShackWave();
    }

    /// <summary>
    /// cherche tout les éléments autour pour les pousser...
    /// </summary>
    private void CreateShackWave()
    {
        //par rapport à la position de la balle qui attaque, son radius, et recherche les layers voulu
        Collider[] ToPush = Physics.OverlapSphere(ballRef.transform.position, radius, LayerMask.GetMask(layerToPush));

        //parcourt chaque collider trouvé
        for (int i = 0; i < ToPush.Length; i++)
        {
            Collider toPush = ToPush[i];
            //si ce collider n'est pas notre ball attaquante...
            if (toPush.gameObject.GetInstanceID() != ballRef.gameObject.GetInstanceID())
            {
                //pousser ! Trouver le vecteur direction ball - collider, et ajouter une force au collider
                Vector3 forceDirection = ballRef.transform.position - toPush.transform.position;
                forceDirection = forceDirection.normalized;
                forceDirection.y = 0f;
                Debug.DrawRay(ballRef.transform.position, forceDirection, Color.red, 3f);
                toPush.GetComponent<Rigidbody>().AddForce(forceImpulse * -forceDirection, ForceMode.Impulse);
            }
        }
    }
    #endregion

    #region unity fonction and ending
    /// <summary>
    /// affichage du radius
    /// </summary>
    void OnDrawGizmosSelected()
    {
        if (!ballRef)
            return;
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(ballRef.transform.position, radius);
    }

    #endregion
}
