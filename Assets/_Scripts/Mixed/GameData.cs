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
        AdditiveJustFinishLoad,
    };

    public enum Prefabs
    {
        Link,           //tag & objet link
        LinkSpawner,    //objet de tak Bonus
        BonusTaken,     //particle, sans tag
        BonusText,      //particle, tag score
        DesactiveLink,  //particle, sans tag
        LevelDesign,    //tag level design
        Ball,           //tag ball (layer player)
        ParticleShockWave,  //particle shockwave, sans tag,
    };

    public enum Layers
    {
        Object,         //tout les objets du décors actif (les boules)
        Player,         //les balls
        Rope,           //les link sont dans ce layer
    }
    #endregion
}
