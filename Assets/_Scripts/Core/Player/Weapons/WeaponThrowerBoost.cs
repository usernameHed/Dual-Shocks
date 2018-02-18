using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// description
/// <summary>

//[RequireComponent(typeof(CircleCollider2D))]
public class WeaponThrowerBoost : Weapon
{
    #region variable

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
        SoundManager.GetSingleton.playSound("Thrower" + transform.GetInstanceID().ToString());
    }

    protected override void OnShootHold()
    {
        
    }

    protected override void OnShootRelease()
    {

        SoundManager.GetSingleton.playSound("Thrower" + transform.GetInstanceID().ToString(), true);
    }

    #endregion

    #region unity fonction and ending

    #endregion
}
