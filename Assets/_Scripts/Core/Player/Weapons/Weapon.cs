using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class Weapon : MonoBehaviour
{
    [FoldoutGroup("Gameplay"), Tooltip("cooldown de l'arme"), SerializeField]
    private float cooldown;

    [FoldoutGroup("Object"), Tooltip("ref sur l'animator"), SerializeField]
    protected Animator animator;

    [FoldoutGroup("Debug"), Tooltip("ref sur playerController"), SerializeField]
    protected PlayerController playerRef;
    [FoldoutGroup("Debug"), Tooltip("ref sur la ball"), SerializeField]
    protected Balls ballRef;


    private float nextShoot;

	void LateUpdate()
	{
		if (nextShoot > 0)
		{
			nextShoot -= Time.deltaTime;
		}
	}

	public void TryShoot(float rotation)
	{
		if (nextShoot <= 0)
		{
			nextShoot = cooldown;
			Shoot (rotation);
		}
	}

	abstract protected void Shoot (float rotation);

	public virtual void OnShootRelease(){}

	public virtual float WeaponPercent()
	{
		return Mathf.Clamp((cooldown - nextShoot) / cooldown, 0.0F, 1.0F);
	}

}
