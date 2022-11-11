using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <c>Abstract Class Building</c>, herits from <c><see cref="Target"/></c>
/// Handles construction state. It is an abstract clas as it the mother class for every building
/// Made by : Nebre 38-169
/// Last Update : 09/11/2022 by Nebre 38-169
/// </summary>
public abstract class Building : Target
{
    public bool constructed;
    //Holds the initial material, to reset the material once it is builded.
    public Material initialMat;

    /// <summary>
    /// <c>Function set Constructed</c>
    /// Used when the building is finished to activates its properties
    /// </summary>
    /// <param name="b"></param>
    public void setConstructed(bool b)
    {
        constructed = b;
        if (constructed)
        {
            GetComponentInChildren<Renderer>().material = initialMat;
        }
        else
        {
            GetComponentInChildren<Renderer>().material.color = Color.gray;
        }
    }

    /// <summary>
    /// <c>Function is Constructed</c>
    /// Indicates whether or not the building is finished
    /// </summary>
    /// <returns></returns>
    public bool isConstructed()
    {
        return constructed;
    }
}
