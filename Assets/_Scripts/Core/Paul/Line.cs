﻿using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Line : MonoBehaviour
{
    #region Attributes
    [FoldoutGroup("Debug"), Tooltip("playerController"), SerializeField]
    private PlayerController playerController;
    public PlayerController PlayerControllerVariable { get { return playerController; } }

    private LineRenderer lineRenderer;
    private List<Transform> followersList;  //list personnel du script, copié du playerController
    #endregion

    #region Init
    private void Awake ()
    {
		lineRenderer = GetComponent<LineRenderer>();
	}

    private void Start()
    {
        //une copie unique de la liste en début de partie
        // (pour avoir la liste set à un seul endroit: dans le playerCOntroller au début)
        followersList = playerController.FollowersList; 
    }
    #endregion

    #region core

    #endregion

    /// <summary>
    /// update la position des extrémités de la corde en lateUpdate
    /// (après les calcules des nouvelles positions physiques)
    /// </summary>
    private void LateUpdate ()
    {
		lineRenderer.SetPosition (0, followersList[0].position);
        lineRenderer.SetPosition (1, followersList[1].position);
	}
}