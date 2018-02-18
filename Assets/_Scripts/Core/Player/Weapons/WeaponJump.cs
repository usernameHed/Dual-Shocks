using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Networking;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class WeaponJump : Weapon
{
    #region variable
    /// <summary>
    /// variable
    /// </summary>
    [FoldoutGroup("Gameplay"), Tooltip("Impulsion du joueur lors du tir"), SerializeField]
    private float forceImpulse = 10f;

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
		Debug.Log("Jumpi !");

        ballRef.BallBody.AddForce(Vector3.up * forceImpulse, ForceMode.VelocityChange);
        SoundManager.GetSingleton.playSound("Jump" + transform.GetInstanceID().ToString());
    }
    #endregion

    #region unity fonction and ending

    
    #endregion
}
