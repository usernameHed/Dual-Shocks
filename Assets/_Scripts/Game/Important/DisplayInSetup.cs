using UnityEngine;
using Sirenix.OdinInspector;
using TMPro;
using System;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// DisplayInGame Description
/// </summary>
public class DisplayInSetup : MonoBehaviour
{
    [Serializable]
    public struct PlayerRef
    {
        public List<GameObject> phasePlayer;        //a désactiver si on joue pas
        public Rope rope;
        public PlayerControllerMenu playerMenu;
        public List<Image> power1;
        public List<Image> power2;

        public void ActiveObj(int id)
        {
            for (int i = 0; i < phasePlayer.Count; i++)
            {
                phasePlayer[i].SetActive(i == id);
            }
        }
    }

    #region Attributes
   
    [FoldoutGroup("Objects"), Tooltip("list spawn players"), SerializeField]
    private PlayerRef[] playersSetup = new PlayerRef[playerMax];

    [FoldoutGroup("Objects"), Tooltip("list des icone de pouvoir 1"), SerializeField]
    private List<Sprite> powerImage1;
    [FoldoutGroup("Objects"), Tooltip("list des icone de pouvoir 2"), SerializeField]
    private List<Sprite> powerImage2;


    [FoldoutGroup("Debug"), Tooltip("Debug"), SerializeField]
    private SetupManager setupManager;

    private const int playerMax = 4;  //nombre de ball du joueur

    #endregion

    #region Initialization

    #endregion

    #region Core
    /// <summary>
    /// initialise le display avec les joueurs connecté
    /// </summary>
    public void InitDisplay()
    {
        
    }

    /// <summary>
    /// ici actualise le display des pouvoirs
    /// </summary>
    public void ChangePowerDisplay()
    {
        Debug.Log("ici change le display des weapons");
        for (int i = 0; i < playersSetup.Length; i++)
        {
            //power 1
            playersSetup[i].power1[0].sprite = powerImage1[GameManager.GetSingleton.PlayerBallInit.PlayerData[i].ballInfo[0].powers[0]];
            playersSetup[i].power1[1].sprite = powerImage1[GameManager.GetSingleton.PlayerBallInit.PlayerData[i].ballInfo[1].powers[0]];

            //power 2
            playersSetup[i].power2[0].sprite = powerImage2[GameManager.GetSingleton.PlayerBallInit.PlayerData[i].ballInfo[0].powers[1]];
            playersSetup[i].power2[1].sprite = powerImage2[GameManager.GetSingleton.PlayerBallInit.PlayerData[i].ballInfo[1].powers[1]];
        }
    }

    /// <summary>
    /// actualise le display par rapport aux scores, player, etc
    /// </summary>
    public void ChangeDisplayInGame()
    {
        for (int i = 0; i < setupManager.IdPhaseConnexion.Length; i++)
        {
            int phase = setupManager.IdPhaseConnexion[i];
            switch (phase)
            {
                case 0:
                    playersSetup[i].ActiveObj(0);
                    playersSetup[i].rope.gameObject.SetActive(false);

                    break;
                case 1:
                    playersSetup[i].ActiveObj(1);
                    playersSetup[i].rope.gameObject.SetActive(false);

                    break;
                case 2:
                    playersSetup[i].ActiveObj(2);
                    playersSetup[i].rope.gameObject.SetActive(true);
                    playersSetup[i].playerMenu.EnabledScript = true;
                    ChangePowerDisplay();
                    break;
                case 3:
                    playersSetup[i].playerMenu.EnabledScript = false;
                    playersSetup[i].phasePlayer[3].SetActive(true);
                    break;
            }
        }
    }

    /// <summary>
    /// supprime les Link (les remet dans la pool !) pour la suite...
    /// </summary>
    public void CleanUp()
    {
        for (int i = 0; i < setupManager.IdPhaseConnexion.Length; i++)
        {
            playersSetup[i].rope.ClearJoints();
        }
    }
    #endregion

    #region Unity ending functions

    #endregion
}
