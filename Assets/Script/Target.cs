using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <c>Class Target</c>, herits from MonoBehaviour
/// Handles life, ally and debug functions
/// Made by : Nebre 38-169
/// Last Update : 09/11/2022 by Nebre 38-169
/// </summary>
public abstract class Target : MonoBehaviour
{
    public float maxLife;
    //Indicates wheter or not the target is on player side
    public bool ally;
    public bool debug;

    protected float currentLife;
    protected List<OpponentInterface> opponents;

    protected void Awake()
    {
        currentLife = maxLife;
        opponents = new List<OpponentInterface>();
    }

    /// <summary>
    /// <c>Function add Opponent</c>
    /// Adds an opponent to the opponent list
    /// </summary>
    /// <param name="u"></param>
    public void addOpponent(OpponentInterface u)
    {
        if (!opponents.Contains(u)) { opponents.Add(u); }
    }

    /// <summary>
    /// <c>Function remove Opponent</c>
    /// Removes an opponent from the opponent list
    /// </summary>
    /// <param name="u"></param>
    public void removeOpponent(OpponentInterface u)
    {
        if (opponents.Contains(u)) { opponents.Remove(u); }
    }

    /// <summary>
    /// <c>Function on Death</c>
    /// Handles death of the target, by warning all its opponents of it.
    /// </summary>
    public void onDeath()
    {
        foreach(OpponentInterface u in opponents)
        {
            u.unSetTarget(this);
        }
        Destroy(this.gameObject);
    }

    /// <summary>
    /// <c>Function on Take Damage</c>
    /// Handles damage, if the life goes below zeros the target is dead
    /// </summary>
    /// <param name="d"></param>
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