using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class WeaponThrowerBoost : Weapon
{
    #region variable
    [FoldoutGroup("Gameplay"), Tooltip("Impulsion du joueur lors du dash"), SerializeField]
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
    /// appelé la première fois qu'on appui
    /// </summary>
	protected override bool OnShoot()
    {
        SoundManager.GetSingleton.playSound(GameData.Sounds.Thrower.ToString() + transform.GetInstanceID().ToString());
        ApplyForce();
        return (true);
    }

    /// <summary>
    /// appelé quand on reste appuyé, après OnShoot
    /// (seulement si la variable hold est activé)
    /// </summary>
    protected override void OnShootHold()
    {
        ApplyForce();
    }

    /// <summary>
    /// appelé quand on relache
    /// (seulement si la variable hold est activé)
    /// </summary>
    protected override void OnShootRelease()
    {
        SoundManager.GetSingleton.playSound(GameData.Sounds.Thrower.ToString() + transform.GetInstanceID().ToString(), true);
    }

    /// <summary>
    /// applique la force constante
    /// </summary>
    private void ApplyForce()
    {
        ballRef.BallBody.AddForce(playerRef.FollowersList[ballRef.IdBallPlayer].transform.forward * -forceImpulse * Time.deltaTime, ForceMode.Impulse);
    }

    #endregion

    #region unity fonction and ending

    #endregion
}
