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
    /// variable public
    /// </summary>
    [FoldoutGroup("Gameplay"), Tooltip("Impulsion du joueur lors du tir"), SerializeField]
    private float forceImpulse = 10f;

    /// <summary>
    /// variable privé
    /// </summary>
	//private Rigidbody playerBody;

    #endregion
    
    #region  initialisation

    #endregion

    #region core script
    /// <summary>
    /// functionTest
    /// </summary>
	protected override void Shoot(float rotation)
    {
        //SoundManager.Instance.PlaySound (SoundManager.Instance.RocketLaunchSound);

		Debug.Log("Jumpi !");
        animator.SetBool(0, true);
        ballRef.BallBody.AddForce(transform.up * -forceImpulse, ForceMode.Impulse);
    }
    #endregion

    #region unity fonction and ending

    [FoldoutGroup("Debug")]
    [Button("destroyThis")]
    public void destroyThis()
    {
		//SoundManager.Instance.PlaySound (SoundManager.Instance.RocketSound);
        Destroy(gameObject);
    }
    #endregion
}
