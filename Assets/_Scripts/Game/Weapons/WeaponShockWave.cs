﻿using UnityEngine;
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

    [Space(10)]

    [FoldoutGroup("Gameplay"), Tooltip("Est-ce qu'on pousse les Link ?"), SerializeField]
    private bool pushLink = false;
    [FoldoutGroup("Gameplay"), EnableIf("pushLink"), Tooltip("Est-ce qu'on pousse les ball & rope allié ?"), SerializeField]
    private bool pushFriendLink = false;
    [FoldoutGroup("Gameplay"), EnableIf("pushLink"), Tooltip("Impulsion du joueur lors du tir"), SerializeField]
    private float forceImpulseLink = 1f;

    [Space(10)]
    [FoldoutGroup("Gameplay"), Tooltip("Est-ce qu'on pousse les ball & rope allié ?"), SerializeField]
    private bool pushFriendBall = false;

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
        shockwaveEffect.CreateWave(ballRef.transform);  //créé l'effet de shackwave

        SoundManager.GetSingleton.playSound("ShockWave" + transform.GetInstanceID().ToString());
        CreateShackWave();  //applique les forces
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

            //ne rien faire si c'est l'objet courrant...
            if (toPush.gameObject.GetInstanceID() == ballRef.gameObject.GetInstanceID())
                continue;

            //si on a trouvé un link...
            if (toPush.gameObject.CompareTag("Link"))
            {
                if (!pushLink)  //ne rien faire si on a choisi de ne pas pousser les links...
                    continue;
                //ici on a un link

                //ne rien faire si ce link est amis et !pushFriendLink
                if (!pushFriendLink && playerRef.IsContainingThisLink(toPush.gameObject))
                    continue;

                //ici on a un link enemi (ou allié si pushFriendLink est vrai)
                PushObject(toPush, true);   //dis qu'on applique la force des links !
            }
            //si on a trouvé une ball..
            else if (toPush.gameObject.CompareTag("Ball"))
            {
                if (!pushFriendBall && playerRef.isContainingThisBall(toPush.gameObject))
                    continue;
                //ici on a une ball ennemi (ou allié si pushFriendBall est vrai)
                PushObject(toPush);
            }
            else
            {
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
        //pousser ! Trouver le vecteur direction ball - collider, et ajouter une force au collider
        Vector3 forceDirection = ballRef.transform.position - toPush.transform.position;
        forceDirection = forceDirection.normalized;
        forceDirection.y = 0f;
        Debug.DrawRay(ballRef.transform.position, forceDirection, Color.red, 3f);

        //détermine si on applque la force pour les link ou pas
        float force = (!isLinkForce) ? forceImpulse : forceImpulseLink;

        toPush.GetComponent<Rigidbody>().AddForce(force * -forceDirection, ForceMode.Impulse);
    }
    #endregion

    #region unity fonction and ending
    /// <summary>
    /// affichage du radius dans l'éditeur quand on sélectionne la ball
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
