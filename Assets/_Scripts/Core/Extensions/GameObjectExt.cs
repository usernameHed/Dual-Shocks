using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectExt
{
    //setup un layerMask en enlevant certain layer...
    //int layerMask = ~((1 << LayerMask.NameToLayer("Walls")) | (1 << LayerMask.NameToLayer("Lock")) | (1 << LayerMask.NameToLayer("Ignore Raycast")) );

    /// <summary>
    /// change le layer de TOUT les enfants
    /// </summary>
    //use: myButton.gameObject.SetLayerRecursively(LayerMask.NameToLayer(“UI”));
    public static void SetLayerRecursively(this GameObject gameObject, int layer)
    {
        gameObject.layer = layer;
        foreach (Transform t in gameObject.transform)
            t.gameObject.SetLayerRecursively(layer);
    }

    /// <summary>
    /// activate recursivly the Colliders
    /// use: gameObject.SetCollisionRecursively(false);
    /// </summary>
    public static void SetCollisionRecursively(this GameObject gameObject, bool tf)
    {
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
            collider.enabled = tf;
    }
    /// <summary>
    /// activate recursivly the Visual (render);
    /// use: gameObject.SetVisualRecursively(false);
    /// </summary>
    public static void SetVisualRecursively(this GameObject gameObject, bool tf)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
            renderer.enabled = tf;
    }

    /// <summary>
    /// but what if you want the collision mask to be based on the weapon’s layer?
    /// It’d be nice to set some weapons to “Team1” and others to “Team2”,
    /// perhaps, and also to ensure your code doesn’t break if you change
    /// the collision matrix in the project’s Physics Settings
    /// 
    /// USE:
    /// if(Physics.Raycast(startPosition, direction, out hitInfo, distance,
    ///                          weapon.gameObject.GetCollisionMask()) )
    ///{
    ///    // Handle a hit
    ///}
    ///
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static int GetCollisionMask(this GameObject gameObject, int layer = -1)
    {
        if (layer == -1)
            layer = gameObject.layer;

        int mask = 0;
        for (int i = 0; i < 32; i++)
            mask |= (Physics.GetIgnoreLayerCollision(layer, i) ? 0 : 1) << i;

        return mask;
    }

    /// <summary>
    /// renvoi si le joueur est grounded ou pas
    /// </summary>
    public static bool IsGrounded(GameObject target, float distToGround, float marginDistToGround = 0.1f)
    {
        return Physics.Raycast(target.transform.position, -Vector3.up, distToGround + marginDistToGround);
    }
}
