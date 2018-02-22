using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class PlayerData : PersistantData
{
    #region Attributes

    [FoldoutGroup("GamePlay"), Tooltip("round courrant des joueurs"), SerializeField]
    private int currentRound = 0;
    public int CurrentRound { get { return currentRound; } set { currentRound = value; } }

    [FoldoutGroup("GamePlay"), Tooltip("score des 4 joueurs"), SerializeField]
    private int[] scorePlayer = new int[SizeArrayId];
    public int[] ScorePlayer { get { return scorePlayer; } }

    [FoldoutGroup("GamePlay"), Tooltip("round courrant des joueurs"), SerializeField]
    private int maxRound = 3;

    private const int SizeArrayId = 4;  //nombre de ball du joueur
    #endregion

    #region Core
    /// <summary>
    /// reset toute les valeurs à celle d'origine pour le jeu
    /// </summary>
    public void SetDefault()
    {
        currentRound = maxRound;
        for (int i = 0; i < SizeArrayId; i++)
        {
            scorePlayer[i] = 0;
        }        
    }

    public override string GetFilePath ()
	{
		return "playerData.dat";
	}

	#endregion
}