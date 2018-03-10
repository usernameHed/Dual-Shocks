using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Text.RegularExpressions;

/// <summary>
/// Fonctions utile
/// <summary>

public static class UtilityFunctions
{
    #region core script

    /// <summary>
    /// renvoi l'angle entre deux vecteur, avec le 3eme vecteur de référence
    /// </summary>
    /// <param name="a">vecteur A</param>
    /// <param name="b">vecteur B</param>
    /// <param name="n">reference</param>
    /// <returns>Retourne un angle en degré</returns>
    public static float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n)
    {
        float angle = Vector3.Angle(a, b);                                  // angle in [0,180]
        float sign = Mathf.Sign(Vector3.Dot(n, Vector3.Cross(a, b)));       //Cross for testing -1, 0, 1
        float signed_angle = angle * sign;                                  // angle in [-179,180]
        float angle360 = (signed_angle + 360) % 360;                       // angle in [0,360]
        return (angle360);
    }

    /// <summary>
    /// get random number between 2;
    /// </summary>
    public static int GetRandomNumber(int minimum, int maximum)
    {
        System.Random random = new System.Random();
        return random.Next() * (maximum - minimum) + minimum;
    }

    public static bool IsTargetOnScreen(Camera cam, Transform target, float xMargin = 0, float yMargin = 0, Renderer render = null)
    {
        if (!cam)
            return (false);
        Vector3 boundExtent = (render == null) ? Vector3.zero : render.bounds.extents;

        Vector3 bottomCorner = cam.WorldToViewportPoint(target.position - boundExtent);
        Vector3 topCorner = cam.WorldToViewportPoint(target.position + boundExtent);

        return (topCorner.x >= -xMargin && bottomCorner.x <= 1 + xMargin && topCorner.y >= -yMargin && bottomCorner.y <= 1 + yMargin);
    }

    /// <summary>
    /// si TOTO532, retourne "532"
    /// </summary>
    public static int GetLastNumberFromString(string lastNNumber)
    {
        var x = Regex.Match(lastNNumber, @"([0-9]+)[^0-9]*$");

        if (x.Success && x.Groups.Count > 0)
        {
            int foundNumber = Int32.Parse(x.Groups[1].Captures[0].Value);
            return (foundNumber);
        }
        return (0);
    }

    /// <summary>
    /// prend en parametre un fileName, et renvoi le prochain numéro
    /// </summary>
    public static string GetNextFileName(string fileName)
    {
        string extension = Path.GetExtension(fileName);
        string pathName = Path.GetDirectoryName(fileName);
        string fileNameOnly = Path.Combine(pathName, Path.GetFileNameWithoutExtension(fileName));
        int i = 0;
        // If the file exists, keep trying until it doesn't
        while (File.Exists(fileName))
        {
            i += 1;
            fileName = string.Format("{0}({1}){2}", fileNameOnly, i, extension);
        }
        return fileName.Replace("\\", "/");
    }

    /// <summary>
    /// prend en parametre une liste d'enum, et renvoi un array de string de ces enum !
    /// </summary>
    /// <param name="layers"></param>
    /// <returns></returns>
    public static string[] GetStringsFromEnum<T>(T[] layers)
    {
        string[] toPush = new string[layers.Length];
        for (int i = 0; i < toPush.Length; i++)
        {
            toPush[i] = layers[i].ToString();
        }
        return (toPush);
    }

    /// <summary>
    /// Test si l'objet est dans la range d'un autre
    /// (pour visualiser la range dans l'éditeur, attacher le script DrawSolidArc
    ///  avec les valeur fovRange et fovAngle sur l'objet "first")
    /// </summary>
    /// <param name="first">objet emmeteur (avec son angle et sa range)</param>
    /// <param name="target">l'objet cible à tester</param>
    /// <param name="fovRange">La range du joueur</param>
    /// <param name="fovAngle">l'angle du joueur</param>
    /// <param name="withRaycast">Doit-on utiliser un raycast ?</param>
    /// <param name="layerMask">Si on utilise un raycast, sur quel layer ?</param>
    /// <returns>Retourne si oui ou non l'objet cible est dans la zone !</returns>
    public static bool isInRange(Transform first, Transform target, float fovRange, float fovAngle, bool checkDistance = false, bool withRaycast = false, int layerMask = -1)
    {
        Vector3 B = target.transform.position - first.position;
        Vector3 C = Quaternion.AngleAxis(90 + fovAngle / 2, -first.forward) * -first.right;

        RaycastHit hit;
        if (SignedAngleBetween(first.up, B, first.up) <= SignedAngleBetween(first.up, C, first.up) || fovAngle == 360)
        {
            //on est dans le bon angle !
            //est-ce qu'on check la distance ?
            if (checkDistance)
            {
                //on test la distance mais sans raycast ?
                if (!withRaycast)
                {
                    Vector3 offset = target.position - first.position;
                    float sqrLen = offset.sqrMagnitude;
                    if (sqrLen < fovRange * fovRange)
                        return (true);
                    return (false);
                }
                //on test la distance, puis le raycast ?
                else
                {
                    Vector3 offset = target.position - first.position;
                    float sqrLen = offset.sqrMagnitude;
                    if (sqrLen < fovRange * fovRange)
                    {
                        if (Physics.Raycast(first.position, B, out hit, fovRange * fovRange, layerMask))
                        {
                            if (hit.transform.GetInstanceID() == target.GetInstanceID())
                                return (true);
                        }
                    }
                    return (false);
                }
            }
            //ne check pas la distance !
            else
            {
                //si on ne check pas de raycast, alors on est dans la range !
                if (!withRaycast)
                    return (true);
                //ici on ne check pas la distance, mais le raycast !
                else
                {
                    if (Physics.Raycast(first.position, B, out hit, Mathf.Infinity, layerMask))
                    {
                        if (hit.transform.GetInstanceID() == target.GetInstanceID())
                            return (true);
                    }
                    return (false);
                }
            }
            
        }
        //ici on est pas dans l'angle...
        return (false);
    }
    #endregion
}
