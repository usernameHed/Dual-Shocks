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
    /// functionTest
    /// </summary>
    protected override void OnShoot()
    {
		Debug.Log("Dash");
        SoundManager.GetSingleton.playSound("Swouch" + transform.GetInstanceID().ToString());
        ballRef.BallBody.AddForce(playerRef.FollowersList[ballRef.IdBallPlayer].transform.forward * -forceImpulse, ForceMode.VelocityChange);
        Invoke("Teleport", 0.1f);

    }

    private void Teleport()
    {
        Vector3 forward = playerRef.FollowersList[ballRef.IdBallPlayer].transform.forward * -1;
        ballRef.BallBody.MovePosition(ballRef.BallBody.transform.position + forward * forwardDist);
    }
    #endregion

    #region unity fonction and ending

   
    #endregion
}
