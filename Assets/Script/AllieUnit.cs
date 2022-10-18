using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                //generatePath(u.transform.position);
            }
        }
    }
}
