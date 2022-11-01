using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTrigger : MonoBehaviour
{
    private List<Target> targetInRange;
    private SphereCollider col;

    private void Awake()
    {
        col = GetComponent<SphereCollider>();
        if(col == null) { throw new System.Exception("Missing Sphere Collider on target trigger"); }
        targetInRange = new List<Target>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Target t = other.GetComponent<Target>();
        if(t != null && (other.GetType()==typeof(CapsuleCollider) || other.GetType() == typeof(BoxCollider)))
        {
            if (!targetInRange.Contains(t))
            {
                targetInRange.Add(t);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Target t = other.GetComponent<Target>();
        if (t != null && targetInRange.Contains(t))
        {
            targetInRange.Add(t);
        }
    }

    public void setCollider(float range)
    {
        if(col != null)
        {
            col.radius = range;
        }
    }

    public bool isTargetInRange(Target t)
    {
        return targetInRange.Contains(t);
    }
}

