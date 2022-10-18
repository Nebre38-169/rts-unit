using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseCombatUnit : Unit
{
    public float searchRange = 10f;
    public float attackRange = 2f;
    public float coolDownDuration = 1f;

    private int frameCounter;
    private SphereCollider rangeCollider;
    

    new private void Awake()
    {
        base.Awake();
        rangeCollider = GetComponent<SphereCollider>();
        rangeCollider.radius = searchRange;
    }

    new private void FixedUpdate()
    {
        base.FixedUpdate();
        if(path != null && target != null)
        {
            float targetDistance = Mathf.Abs(Vector3.Distance(transform.position, target.transform.position));
            if(targetDistance <= attackRange)
            {
                onPathComplete();
            }
        }
        if (isTargetInAttackRange() && frameCounter > coolDownDuration*60)
        {
            onDealDamage(target);
        }
        else if (isTargetInAttackRange())
        {
            frameCounter++;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Unit u = other.GetComponent<Unit>();
        if(u != null && target == null)
        {
            target = u;
            generatePath(u.transform.position);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void onDealDamage(Unit u)
    {
        u.onTakeDamage(damage, this);
        frameCounter = 0;
    }

    private bool isTargetInAttackRange()
    {
       if(target != null)
        {
            float targetDistance = Mathf.Abs(Vector3.Distance(transform.position, target.transform.position));
            return targetDistance <= attackRange;
        }
        return false;
    }
}
