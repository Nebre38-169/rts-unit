using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Target : MonoBehaviour
{
    public float maxLife;
    public bool ally;
    public bool debug;

    protected float currentLife;
    protected List<Unit> opponents;

    protected void Awake()
    {
        currentLife = maxLife;
        opponents = new List<Unit>();
    }

    public void addOpponent(Unit u)
    {
        if (!opponents.Contains(u)) { opponents.Add(u); }
    }

    public void removeOpponent(Unit u)
    {
        if (opponents.Contains(u)) { opponents.Remove(u); }
    }

    public void onDeath()
    {
        foreach(Unit u in opponents)
        {
            u.unSetTarget();
        }
        Destroy(this.gameObject);
    }

    public void onTakeDamage(float d)
    {
        currentLife-= d;
        if (currentLife <= 0) { onDeath(); }
    }

    /// <summary>
    /// Used for debug
    /// </summary>
    /// <param name="message"></param>
    protected void debugMessage(string message)
    {
        if (debug) { Debug.Log(message); }
    }

}