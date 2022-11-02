using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Building, OpponentInterface
{
    public float attackRange;
    public float damage;
    public float coolDownDuration;

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
