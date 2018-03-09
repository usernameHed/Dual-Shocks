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
        LinkBonus,    //objet de tak Bonus
        BonusTaken,     //particle, sans tag
        BonusText,      //particle, tag score
        DesactiveLink,  //particle, sans tag
        LevelDesign,    //tag level design
        Ball,           //tag ball (layer player)
        ParticleShockWave,  //particle shockwave, sans tag,
        Enemy,          //enemy
        EnemyExplode,   //particle
        BallExplode,    //particle ball explode
        Blockers,
    };

    public enum Layers
    {
        Object,         //tout les objets du décors actif (les boules)
        Player,         //les balls
        Rope,           //les link sont dans ce layer
    }

    /// <summary>
    /// retourne vrai si le layer est dans la list
    /// </summary>
    public static bool IsInList(List<Layers> listLayer, int layer)
    {
        string layerName = LayerMask.LayerToName(layer);
        for (int i = 0; i < listLayer.Count; i++)
        {
            if (listLayer[i].ToString() == layerName)
            {
                return (true);
            }
        }
        return (false);
    }
    #endregion
}
