using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

/// <summary>
/// Fonctions utile
/// <summary>
public static class GameData
{
    #region core script

    public enum Event
    {
        PlayerDeath,
        GameOver,
        GamePadConnectionChange,
    };

    public enum Prefabs
    {
        Link,
        LinkSpawner,
        BonusTaken,
        BonusText,
        DesactiveLink,
    };

    /*public static float SignedAngleBetween()
    {

    }*/

    #endregion
}
