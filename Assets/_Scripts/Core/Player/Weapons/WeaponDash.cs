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
    [FoldoutGroup("Gameplay"), Tooltip("distance de téléportation"), SerializeField]
    private float forwardDist = 3f;
    [FoldoutGroup("Gameplay"), Tooltip("Temps d'attente entre la poussé et le dash"), SerializeField]
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
        ballRef.BallBody.AddForce(playerRef.FollowersList[ballRef.IdBallPlayer].transform.forward * -forceImpulse, ForceMode.VelocityChange);

        //ici attend timeToCallTeleport seconde, puis téléporte
        Invoke("Teleport", timeToCallTeleport);

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
