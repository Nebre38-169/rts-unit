using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <c>Class Turret</c>, herits from <c><see cref="Building"/></c> and implements <c><see cref="OpponentInterface"/></c>
/// This building is a defense one. It shoots arrow at incoming enemy.
/// It uses a Sphere Collider to detect enemy and shoot them until they die.
/// Made by : Nebre 38-169
/// Last Update : 09/11/2022 by Nebre 38-169
/// </summary>
public class Turret : Building, OpponentInterface
{
    public float attackRange;
    public float damage;
    //Time between two arrow
    public float coolDownDuration;

    //Holds every unit in range
    private List<Target> target;
    private SphereCollider attackCollider;
    private float frameCounter;

    new protected void Awake()
    {
        base.Awake();
        attackCollider = GetComponent<SphereCollider>();
        if(attackCollider == null) { throw new SystemException("Missing SphereCollider on " + gameObject.name); }
        attackCollider.radius = attackRange;
        target = new List<Target>();
        frameCounter = 0;
    }

    private void FixedUpdate()
    {
        //If there are enemy in range, we shoot them or increase the counter
        if(target.Count > 0 && frameCounter > coolDownDuration * 60)
        {
            target[0].onTakeDamage(damage);
            frameCounter = 0;
        }
        else if(target.Count > 0)
        {
            frameCounter++;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //We filter collider by spherical one and by unit.
        //Also we only look for enemy unit
        if(other.GetType() == typeof(CapsuleCollider))
        {
            Unit unit = other.GetComponent<Unit>();
            if (unit != null && Utils.isEnemy(this, unit))
            {
                if (!target.Contains(unit))
                {
                    debugMessage("Target acquired !");
                    target.Add(unit);
                    unit.addOpponent(this);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //When an enemy unit leave the attack range, we remove it from the target list
        if (other.GetType() == typeof(CapsuleCollider))
        {
            Unit unit = other.GetComponent<Unit>();
            if (unit != null && Utils.isEnemy(this, unit))
            {
                if (target.Contains(unit))
                {
                    debugMessage("Target lost !");
                    target.Remove(unit);
                    unit.removeOpponent(this);
                }
            }
        }
    }

    /// <summary>
    /// <c>Function unset Target</c>
    /// Removes a unit from the target list
    /// </summary>
    /// <param name="t"></param>
    public void unSetTarget(Target t)
    {
        if (target.Contains(t)) { target.Remove(t); }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    
}
