using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public enum Order
{
    IDLE,
    MOVE,
    ATTACK
}

/// <summary>
/// <para><c>Class Unit</c>, herits from <c>MonoBehaviour</c></para>
/// Handle selected state and movement for a selectable and mobile unit
/// </summary>
public class Unit : MonoBehaviour
{
    public float speed = 5f;
    //This argument is use to smooth the path
    public float nextWaypointDistance = 0.2f;
    public float currentLife;
    public float maxLife = 10f;
    public float damage = 5f;
    public float searchRange = 10f;
    public float attackRange = 2f;
    public float lostRange = 20f;
    public float coolDownDuration = 10f;

    //Store path and state of the unit on the path
    protected Path path;
    private int currentWaypoint = 0;
    //Used to generate the path
    private Seeker seeker;
    //Used to handle movement
    protected Rigidbody rb;
    //Could be used to render a selected state
    //private SpriteRenderer m_SpriteRenderer;
    protected Unit target;
    public Order currentOrder;
    private int frameCounter;
    private SphereCollider rangeCollider;


    protected void Awake()
    {
        //m_SpriteRenderer = GetComponent<SpriteRenderer>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody>();
        setSelected(false);
        currentLife = maxLife;
        currentOrder = Order.IDLE;
        rangeCollider = GetComponent<SphereCollider>();
        rangeCollider.radius = searchRange;
    }

    protected void FixedUpdate()
    {
        if(currentOrder == Order.IDLE && target != null)
        {
            currentOrder = Order.ATTACK;
        }
        if(currentOrder == Order.ATTACK)
        {
            if (isTargetInRange())
            {
                //Debug.Log("Target in range");
                onPathComplete();
                if(frameCounter > coolDownDuration * 60)
                {
                    dealDamage(target);
                }
                else
                {
                    frameCounter++;
                }
            }
            else
            {
                //Debug.Log("Target not in range");
                if(path == null || !isTargetAtEndOfPath()) { generatePath(target.transform.position); }
                moveAlongPath();
            }
        }
        if(currentOrder == Order.MOVE)
        {
            moveAlongPath();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Unit u = other.GetComponent<Unit>();
        if (u != null && target == null)
        {
            target = u;
            if (currentOrder != Order.MOVE)
            {
                currentOrder = Order.ATTACK;
                generatePath(u.transform.position);
            }

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lostRange);
    }

    private void moveAlongPath()
    {
        if(path != null)
        {
            //If there is a path and our current waypoint is the last one, we stop
            if (currentWaypoint >= path.vectorPath.Count)
            {
                onPathComplete();
                return;
            }

            //Calcul speed toward the next waypoint
            Vector3 direction = (path.vectorPath[currentWaypoint] - rb.position).normalized;
            Vector3 force = direction * speed * Time.deltaTime;
            rb.velocity = new Vector3(force.x, 0, force.z);

            //If we are close enough to the next waypoint, we increase the current waypoint counter
            float distance = Vector3.Distance(rb.position, path.vectorPath[currentWaypoint]);
            if (distance < nextWaypointDistance)
            {
                currentWaypoint++;
            }
        }
    }

    /// <summary>
    /// <c>Function generatePath</c>
    /// Create a path to the endPosition using the Seeker.
    /// </summary>
    /// <param name="endPosition">Vector3 the final position the unit must reach</param>
    public void generatePath(Vector3 endPosition)
    {
        //Debug.Log("Calculating path");
        seeker.StartPath(rb.position, endPosition, onPathCalcul);
    }

    public void onTakeDamage(float d, Unit origin)
    {
        currentLife -= d;
        Debug.Log(gameObject.name + " toke " + d + " damage(s)");
        if (currentLife <= 0) {
            origin.onTargetDeath();
            onDeath(); 
        }
    }

    public void onTargetDeath()
    {
        target = null;
        currentOrder = Order.IDLE;
    }

    /// <summary>
    /// <c>Function setSelected</c>
    /// Handle animation selection (when it is developt)
    /// </summary>
    /// <param name="selected">Boolean that indicates wether or not the unit is selected</param>
    public void setSelected(bool selected)
    {
        if (selected)
        {
            Debug.Log(gameObject.name + " is selected");
            //m_SpriteRenderer.color = Color.blue;
        }
        else
        {
            Debug.Log(gameObject.name + " is unselected");
            //m_SpriteRenderer.color = Color.white;
        }
    }

    public void setDestination(Vector3 dest)
    {
        currentOrder = Order.MOVE;
        generatePath(dest);
    }

    public void onPathComplete()
    {
        //Debug.Log("We reached end of path");
        //rb.velocity = Vector3.zero;
        path = null;
        currentWaypoint = 0;
        if(currentOrder == Order.MOVE) { currentOrder = Order.IDLE; }
    }

    /// <summary>
    /// <para>Function onPathComplete</para>
    /// This function is called when the Seeker has finish calculting the path.
    /// It stores the path and reset de waypoint counter
    /// </summary>
    /// <param name="p">Path given by the Seeker</param>
    private void onPathCalcul(Path p)
    {
        //Debug.Log("Path calcul completed");
        //Debug.Log(p.error);
        if (p.error == false)
        {
            //Debug.Log("Path is assigned");
            path = p;
            currentWaypoint = 0;
        }
    }

 

    private void onDeath()
    {
        Destroy(this.gameObject);
    }

    private bool isTargetInRange()
    {
        if (target != null)
        {
            float targetDistance = Mathf.Abs(Vector3.Distance(transform.position, target.transform.position));
            return targetDistance <= attackRange;
        }
        return false;
    }
    private bool isTargetLost()
    {
        if (target != null)
        {
            float targetDistance = Mathf.Abs(Vector3.Distance(transform.position, target.transform.position));
            return targetDistance >= lostRange;
        }
        else
        {
            return false;
        }
    }

    private bool isTargetAtEndOfPath()
    {
        if(path != null)
        {
            float d = Mathf.Abs(Vector3.Distance(target.transform.position,
                path.vectorPath[path.vectorPath.Count - 1]));
            return d < 2;
        }
        else
        {
            return false;
        }
    }

    private void dealDamage(Unit u)
    {
        u.onTakeDamage(damage, this);
        frameCounter = 0;
    }



}
