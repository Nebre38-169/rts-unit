using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
                //generatePath(u.transform.position);
            }

        }
    }
}
