using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para><c>Class Enemy Unit</c>, herits from <c>Unit</c></para>
/// Handle selecting a target, by onlt targeting AllieUnit
/// Made by : Nebre 38-169
/// Last Update : 25/10/2022 by Nebre 38-169
/// </summary>
public class EnemyUnit : Unit
{
    private void OnTriggerEnter(Collider other)
    {
        AllieUnit u = other.GetComponent<AllieUnit>();
        if (u != null && target == null)
        {
            target = u;
            target.addOpponent(this);
            if (currentOrder != Order.MOVE)
            {
                currentOrder = Order.ATTACK;
            }

        }
    }
}
