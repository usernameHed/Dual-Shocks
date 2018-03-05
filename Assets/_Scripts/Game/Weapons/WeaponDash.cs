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
    /// variable
    /// </summary>
    [FoldoutGroup("Gameplay"), Tooltip("Impulsion du joueur lors du dash"), SerializeField]
    private float forceImpulse = 10f;
    [FoldoutGroup("Gameplay"), Tooltip("Impulsion du joueur lors du tir"), SerializeField]
    private float forceLinkImpulse = 10f;
    [FoldoutGroup("Gameplay"), Range(0, 100), Tooltip("Nombre de link à pousser aussi"), SerializeField]
    private int linkToPush = 0;

    [FoldoutGroup("Gameplay"), Tooltip("Téléporte le joueur ?"), SerializeField]
    private bool teleport = true;

    [FoldoutGroup("Gameplay"), EnableIf("teleport"), Tooltip("distance de téléportation"), SerializeField]
    private float forwardDist = 3f;
    [FoldoutGroup("Gameplay"), EnableIf("teleport"), Tooltip("Temps d'attente entre la poussé et le dash"), SerializeField]
    private float timeToCallTeleport = 0.1f;

    #endregion

    #region private variable

    #endregion

    #region  initialisation

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
    /// Applique une force qui pousse la ball devant lui, selon la fariable forceImpulse
    /// </summary>
    protected override void OnShoot()
    {
		Debug.Log("Dash");
        SoundManager.GetSingleton.playSound("Swouch" + transform.GetInstanceID().ToString());

        //pousse la balle
        ApplyForce(ballRef.BallBody, forceImpulse);
        //ballRef.BallBody.AddForce(playerRef.FollowersList[ballRef.IdBallPlayer].transform.forward * -forceImpulse, ForceMode.VelocityChange);

        //pousse les X link aussi
        if (linkToPush > 0)
            PushLink();

        //ici attend timeToCallTeleport seconde, puis téléporte
        if (teleport)
            Invoke("Teleport", timeToCallTeleport);
    }

    /// <summary>
    /// pousse les X link attaché à la ball
    /// </summary>
    private void PushLink()
    {
        Rope rope = playerRef.RopeScript;

        int startLoop = 0;
        int stopLoop = 0;
        int numberLink = (linkToPush * rope.LinkCircular.Count) / 100;

        //si la ball de référence est au début de la chaine, partir du bas vers le haut de la list !
        if (rope.LinkCircular[0].Value.GetInstanceID() == ballRef.gameObject.GetInstanceID())
        {
            stopLoop = Mathf.Min(numberLink, rope.LinkCircular.Count - 1);
            for (int i = 1; i < stopLoop; i++)
            {
                ApplyForce(rope.LinkCircular[i].Value.GetComponent<Rigidbody>(), forceLinkImpulse);
            }
        }
        //si la ball de référence est à la fin, partir du haut vers le bas
        else if (rope.LinkCircular[rope.LinkCircular.Count - 1].Value.GetInstanceID() == ballRef.gameObject.GetInstanceID())
        {
            startLoop = rope.LinkCircular.Count - 2;
            stopLoop = Mathf.Max(0, (rope.LinkCircular.Count - 1) - numberLink);
            for (int i = startLoop; i > stopLoop; i--)
            {
                ApplyForce(ballRef.BallBody, forceLinkImpulse);
            }
        }

    }

    /// <summary>
    /// applique la force sur la ball / le link
    /// </summary>
    private void ApplyForce(Rigidbody rbToPush, float force)
    {
        rbToPush.AddForce(playerRef.FollowersList[ballRef.IdBallPlayer].transform.forward * -force, ForceMode.VelocityChange);
    }

    /// <summary>
    /// téléporte devant, selon la variable forwardDist
    /// </summary>
    private void Teleport()
    {
        Vector3 forward = playerRef.FollowersList[ballRef.IdBallPlayer].transform.forward * -1;
        ballRef.BallBody.MovePosition(ballRef.BallBody.transform.position + forward * forwardDist);
    }
    #endregion

    #region unity fonction and ending

   
    #endregion
}
