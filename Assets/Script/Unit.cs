using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public enum Order
{
    IDLE,
    MOVE,
    ATTACK,
    FORCEATTACK
}

/// <summary>
/// <para><c>Class Unit</c>, herits from <c>MonoBehaviour</c></para>
/// Handle selected state and movement for a selectable and mobile unit
/// </summary>
public abstract class Unit : MonoBehaviour
{
    public float speed = 5f;
    //This argument is use to smooth the path
    public float nextWaypointDistance = 0.2f;
    //Variable used for combat
    public float currentLife;
    public float maxLife = 10f;
    public float damage = 5f;
    //Variable used for target selection
    public float searchRange = 10f; //Within this range, an enemy will be picked as a target
    public float attackRange = 2f; //Within this range, an enemy will take damage
    public float lostRange = 20f; //Outside this range, a target will be lost and focused will be lost
    public float coolDownDuration = 10f; //Time between two attacks

    //Store path and state of the unit on the path
    protected Path path;
    private int currentWaypoint = 0;
    //Used to generate the path
    private Seeker seeker;
    //Used to handle movement
    protected Rigidbody rb;
    //Could be used to render a selected state
    //private SpriteRenderer m_SpriteRenderer;
    //Private storage for order and target focus
    protected Unit target;
    public Order currentOrder;
    private int frameCounter;
    private SphereCollider rangeCollider;
    //Used to store opponent (unit that store this instance as a target)
    private List<Unit> opponent;


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
        opponent = new List<Unit>();
    }

    protected void FixedUpdate()
    {
        //If the unit is idle and it has picked a target, it goes into combat mode
        if(currentOrder == Order.IDLE && target != null)
        {
            currentOrder = Order.ATTACK;
        }
        if(currentOrder == Order.ATTACK || currentOrder == Order.FORCEATTACK)
        {
            /* When in combat mode, the unit will try to reach its target
             * and when in range, will deal damage between cooldown
             */
            if (targetVerification())
            {
                if (isTargetInRange())
                {
                    //If the unit is in attack range, we assume the path is completed
                    onPathComplete();
                    if (frameCounter > coolDownDuration * 60)
                    {
                        dealDamage(target);
                    }
                    else
                    {
                        frameCounter++;
                    }
                }
                else if (isTargetLost() && currentOrder != Order.FORCEATTACK)
                {
                    //If the target is lost, the unit lose focus and idle
                    unSetTarget();
                    onPathComplete();
                    currentOrder = Order.IDLE;
                }
                else
                {
                    /* We check if the target as move from the original destination
                     * If it's true, we recalculate the path to the target
                     * Else we move along the path
                     */
                    if (path == null || !isTargetAtEndOfPath()) { generatePath(target.transform.position); }
                    moveAlongPath();
                }
            }
        }
        if(currentOrder == Order.MOVE)
        {
            moveAlongPath();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, lostRange);
    }

    /// <summary>
    /// <para><c>Function moveAlongPath</c></para>
    /// Handle unit's movement along a precalculated path
    /// If the unit has no path, do nothing
    /// </summary>
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

    /// <summary>
    /// <para><c>Function onTakeDamage</c></para>
    /// Handle damage given by an other unit.
    /// If the current life goes below zero, the unit is killed and all its oppenent are warn of it
    /// </summary>
    /// <param name="d">Float that represent the damage dealed</param>
    public void onTakeDamage(float d)
    {
        currentLife -= d;
        Debug.Log(gameObject.name + " toke " + d + " damage(s)");
        if (currentLife <= 0) {
            onDeath(); 
        }
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

    /// <summary>
    /// <para><c>Function setDestination</c></para>
    /// Handle setting a destination to a unit.
    /// It also modifies the order to a move order
    /// (we assume the order is given either by the player or by the computer)
    /// </summary>
    /// <param name="dest">Vector3 of the final destination</param>
    public void setDestination(Vector3 dest)
    {
        currentOrder = Order.MOVE;
        generatePath(dest);
    }

    /// <summary>
    /// <para><c>Function setTarget</c></para>
    /// Handle setting a unit as the target.
    /// We use the force attack state to ignore the lost range.
    /// We assume the order is given either by the player or by the computer.
    /// </summary>
    /// <param name="u">The target setted either by the GameRTSController(Player) or by a group intelligence</param>
    public void setTarget(Unit u)
    {
        Debug.Log("unit " + u.gameObject.name + " is the new target");
        currentOrder = Order.FORCEATTACK;
        target = u;
        target.addOpponent(this);
    }

    public void unSetTarget()
    {
        target = null;
        currentOrder = Order.IDLE;
    }

    public void onPathComplete()
    {
        //Debug.Log("We reached end of path");
        rb.velocity = Vector3.zero;
        path = null;
        currentWaypoint = 0;
        if(currentOrder == Order.MOVE) { currentOrder = Order.IDLE; }
    }

    public void addOpponent(Unit u)
    {
        opponent.Add(u);
    }

    public void removeOpponent(Unit u)
    {
        if (opponent.Contains(u))
        {
            opponent.Remove(u);
        }
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
        foreach(Unit u in opponent)
        {
            u.unSetTarget();
        }
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

    private bool targetVerification()
    {
        if(target.gameObject == null) 
        {
            unSetTarget();
            return false;
        }
        return true;
    }

    private void dealDamage(Unit u)
    {
        u.onTakeDamage(damage);
        frameCounter = 0;
    }



}
