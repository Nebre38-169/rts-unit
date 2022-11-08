using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Building : Target
{
    public bool constructed;
    public Material initialMat;

    public void setConstructed(bool b)
    {
        constructed = b;
        if (constructed)
        {
            Debug.Log("Constructed");
            GetComponentInChildren<Renderer>().material = initialMat;
        }
        else
        {
            GetComponentInChildren<Renderer>().material.color = Color.gray;
        }
    }

    public bool isConstructed()
    {
        return constructed;
    }
}
