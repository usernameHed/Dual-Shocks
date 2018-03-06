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
        RoundStart,     //appelé par LevelManager quand le round commence
        PlayerDeath,    //est appelé a chaque playerDeath
        GameOver,       //est appelé quand on trigger un gameOver
        GamePadConnectionChange,    //est appelé a chaque co/deco de manette
    };

    public enum Prefabs
    {
        Link,
        LinkSpawner,
        BonusTaken,
        BonusText,
        DesactiveLink,
        LevelDesign
    };

    /*public static float SignedAngleBetween()
    {

    }*/

    #endregion
}
