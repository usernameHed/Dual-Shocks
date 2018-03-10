using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Networking;
using System.Collections.Generic;

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
    [FoldoutGroup("Gameplay"), Tooltip("Temps de stun si on shock une wave"), SerializeField]
    private float timeStunPlayer = 1f;

    [Space(10)]

    [FoldoutGroup("Gameplay"), Tooltip("Est-ce qu'on pousse les Link ?"), SerializeField]
    private bool pushLink = false;
    [FoldoutGroup("Gameplay"), EnableIf("pushLink"), Tooltip("Est-ce qu'on pousse les ball & rope allié ?"), SerializeField]
    private bool pushFriendLink = false;
    [FoldoutGroup("Gameplay"), EnableIf("pushLink"), Tooltip("Impulsion du joueur lors du tir"), SerializeField]
    private float forceImpulseLink = 1f;

    [FoldoutGroup("Gameplay"), Tooltip("La force est-elle dépendante de la distance ?"), SerializeField]
    private bool distanceDependent = true;

    [FoldoutGroup("Gameplay"), EnableIf("distanceDependent"), Tooltip("multiplie la force par la distance"), SerializeField]
    private float distanceAmplify = 1.5f;

    [Space(10)]
    [FoldoutGroup("Gameplay"), Tooltip("Est-ce qu'on pousse les ball & rope allié ?"), SerializeField]
    private bool pushFriendBall = false;

    [FoldoutGroup("Gameplay"), Tooltip("Nom des layers à chercher et à pousser"), SerializeField]
    private GameData.Layers[] layerToTest;

    private ShowArrow showArrow;

    #endregion

    #region  initialisation
    private void Awake()
    {
        showArrow = GetComponent<ShowArrow>();
        //shockwaveEffect = GetComponent<ShockWaveEffect>();
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
	protected override bool OnShoot()
    {
        Debug.Log("shockwave !");

        ObjectsPooler.Instance.SpawnFromPool(GameData.Prefabs.ParticleShockWave, ballRef.transform.position, Quaternion.identity, ObjectsPooler.Instance.transform);
        SoundManager.GetSingleton.playSound(GameData.Sounds.ShockWave.ToString() + transform.GetInstanceID().ToString());
        CreateShackWave();  //applique les forces
        return (true);
    }

    /// <summary>
    /// cherche tout les éléments autour pour les pousser...
    /// </summary>
    private void CreateShackWave()
    {
        //par rapport à la position de la balle qui attaque, son radius, et recherche les layers voulu
        Collider[] ToPush = Physics.OverlapSphere(ballRef.transform.position, radius, LayerMask.GetMask(UtilityFunctions.GetStringsFromEnum(layerToTest)));

        //parcourt chaque collider trouvé
        for (int i = 0; i < ToPush.Length; i++)
        {
            Collider toPush = ToPush[i];

            //ne rien faire si c'est l'objet courrant...
            if (toPush.gameObject.GetInstanceID() == ballRef.gameObject.GetInstanceID())
                continue;

            //si on a trouvé un link...
            if (toPush.gameObject.CompareTag(GameData.Prefabs.Link.ToString()))
            {
                if (!pushLink)  //ne rien faire si on a choisi de ne pas pousser les links...
                    continue;
                //ici on a un link

                //ne rien faire si ce link est amis et !pushFriendLink
                if (!pushFriendLink && playerRef.RopeScript.IsContainingThisLink(toPush.gameObject))
                    continue;

                //ici on a un link enemi (ou allié si pushFriendLink est vrai)
                PushObject(toPush, true);   //dis qu'on applique la force des links !
            }
            //si on a trouvé une ball..
            else if (toPush.gameObject.CompareTag(GameData.Prefabs.Ball.ToString()))
            {
                if (!pushFriendBall && playerRef.isContainingThisBall(toPush.gameObject))
                    continue;

                Balls ball = toPush.gameObject.GetComponent<Balls>();
                ball.Stun(true, timeStunPlayer);

                //ici on a une ball ennemi (ou allié si pushFriendBall est vrai)
                PushObject(toPush);
            }
            else
            {
                if (toPush.gameObject.layer == LayerMask.NameToLayer(GameData.Layers.Object.ToString()))
                {
                    PushObject(toPush);
                }
                //ici on a un autre objet
                //PushObject(toPush);
            }

        }
    }

    /// <summary>
    /// ici pousse l'objet
    /// </summary>
    private void PushObject(Collider toPush, bool isLinkForce = false)
    {
        Rigidbody bodyToPush = toPush.GetComponent<Rigidbody>();
        if (!bodyToPush)
            return;

        //pousser ! Trouver le vecteur direction ball - collider, et ajouter une force au collider
        Vector3 forceDirection = ballRef.transform.position - toPush.transform.position;
        forceDirection = forceDirection.normalized;
        forceDirection.y = 0f;
        Debug.DrawRay(ballRef.transform.position, forceDirection, Color.red, 3f);

        //détermine si on applque la force pour les link ou pas
        float force = (!isLinkForce) ? forceImpulse : forceImpulseLink;
        if (distanceDependent)
        {
            Debug.Log("force before: " + force);
            float dist = Vector3.Distance(toPush.transform.position, ballRef.transform.position);
            force = ((radius - dist) * force / radius) * distanceAmplify;

            Debug.Log("force before: " + force);
        }


        toPush.GetComponent<Rigidbody>().AddForce(force * -forceDirection, ForceMode.Impulse);
    }
    #endregion

    #region unity fonction and ending

    /// <summary>
    /// affichage du radius dans l'éditeur quand on sélectionne la ball
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    /// <summary>
    /// appelé quand on meurt...
    /// </summary>
    protected override void KillParticularWeapon()
    {
        showArrow.Kill();
    }

    #endregion
}
