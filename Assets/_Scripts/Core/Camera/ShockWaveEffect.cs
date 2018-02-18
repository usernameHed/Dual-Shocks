using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// ShockWaveEffect Description
/// </summary>
public class ShockWaveEffect : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("Gameplay"), Tooltip("l'id des shockwave de script RippleEffect sur la caméra utilisable par ce script"), SerializeField]
    public List<int> idShackwave;

    [FoldoutGroup("Debug"), Tooltip("la main.camera"), SerializeField]
    private GameObject mainCamObject;

    private List<RippleEffect> rippleList = new List<RippleEffect>();
    
    #endregion

    #region Initialization

    private void Start()
    {
        if (!mainCamObject)
        {
            mainCamObject = Camera.main.gameObject;
        }
        SetupRipple();
    }

    private void SetupRipple()
    {
        RippleEffect[] ripples = mainCamObject.GetComponents<RippleEffect>();
        for (int i = 0; i < idShackwave.Count; i++)
        {
            rippleList.Add(ripples[i]);
        }
    }
    #endregion

    #region Core
    /// <summary>
    /// lance un shockwave à une position donnée, par rapport à un transform
    /// </summary>
    public void CreateWave(Transform pos, int id = 0)
    {
        Camera cam = Camera.main;
        Vector2 ViewportPosition = cam.WorldToViewportPoint(pos.position);
        CreateWave(ViewportPosition, id);
    }
    public void CreateWave(Vector2 pos, int id = 0)
    {
        rippleList[id].StartPlay(pos);
    }
    #endregion

    #region Unity ending functions

    #endregion
}
