using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData : PersistantData
{
	#region Attributes

	[Tooltip("La progression du joueur")]
	public int scorePlayer;

    #endregion

    #region Core
    /// <summary>
    /// reset toute les valeurs à celle d'origine pour le jeu
    /// </summary>
    public void SetDefault()
    {
        scorePlayer = 0;
    }

    public override string GetFilePath ()
	{
		return "playerData.dat";
	}

	#endregion
}