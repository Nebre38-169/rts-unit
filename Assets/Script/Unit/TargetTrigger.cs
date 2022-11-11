using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// <c>Class Target Trigger</c>, herits from <c>MonoBehaviour</c>
/// Used by unit to know if their target is in range
/// Made by : Nebre 38-169
/// Last Update : 09/11/2022 by Nebre 38-169
/// </summary>
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
        //When a target enters the collider, wee check for which collider was detected
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
            targetInRange.Remove(t);
        }
    }

    /// <summary>
    /// <c>Function set Collider</c>
    /// Set the range of the collider
    /// </summary>
    /// <param name="range"></param>
    public void setCollider(float range)
    {
        if(col != null)
        {
            col.radius = range;
        }
    }

    /// <summary>
    /// <c>Function is Target In Range</c>
    /// Indicates wheter or not the target is in range
    /// </summary>
    /// <param name="t"></param>
    /// <returns>True if the target is in range, false otherwise</returns>
    public bool isTargetInRange(Target t)
    {
        return targetInRange.Contains(t);
    }
}

