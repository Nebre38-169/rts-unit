using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// <c>Interface Ressource Observer</c>
/// Used for the Observer pattern on ressource update
/// Made by : Nebre 38-169
/// Last Update : 09/11/2022 by Nebre 38-169
/// </summary>
public interface RessourceObserver
{
    /// <summary>
    /// <c>Function on Ressource Update</c>
    /// Used to warn the observer of a ressource change
    /// </summary>
    /// <param name="quantities"></param>
    public void onRessourceUpdate(IDictionary<Ressource, int> quantities);
}
