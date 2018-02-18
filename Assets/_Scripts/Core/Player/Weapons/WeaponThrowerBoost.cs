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
    /// appelé la première fois qu'on appui
    /// </summary>
	protected override void OnShoot()
    {
        SoundManager.GetSingleton.playSound("Thrower" + transform.GetInstanceID().ToString());
    }

    /// <summary>
    /// appelé quand on reste appuyé, après OnShoot
    /// (seulement si la variable hold est activé)
    /// </summary>
    protected override void OnShootHold()
    {
        
    }

    /// <summary>
    /// appelé quand on relache
    /// (seulement si la variable hold est activé)
    /// </summary>
    protected override void OnShootRelease()
    {

        SoundManager.GetSingleton.playSound("Thrower" + transform.GetInstanceID().ToString(), true);
    }

    #endregion

    #region unity fonction and ending

    #endregion
}
