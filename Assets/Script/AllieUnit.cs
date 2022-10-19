using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <para><c>Class Allie Unit</c>, herits from <c>Unit</c></para>
/// Handle selecting a target, by onlt targeting EnemyUnit
/// </summary>
public class AllieUnit : Unit
{
    private void OnTriggerEnter(Collider other)
    {
        EnemyUnit u = other.GetComponent<EnemyUnit>();
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
