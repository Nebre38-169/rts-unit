using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

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


    protected void Awake()
    {
        //m_SpriteRenderer = GetComponent<SpriteRenderer>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody>();
        setSelected(false);
        currentLife = maxLife;
    }

    protected void FixedUpdate()
    {
        //If there is no path, do nothing
        //Debug.Log(path);
        //Debug.Log(currentWaypoint);
        if (path == null)
        {
            //Debug.Log("This bit is executed");
            rb.velocity = Vector3.zero;
            return;
        } 
        else
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
        Debug.Log("Calculating path");
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

    public void onPathComplete()
    {
        Debug.Log("We reached end of path");
        rb.velocity = Vector3.zero;
        path = null;
        currentWaypoint = 0;
    }

    /// <summary>
    /// <para>Function onPathComplete</para>
    /// This function is called when the Seeker has finish calculting the path.
    /// It stores the path and reset de waypoint counter
    /// </summary>
    /// <param name="p">Path given by the Seeker</param>
    private void onPathCalcul(Path p)
    {
        Debug.Log("Path calcul completed");
        Debug.Log(p.error);
        if (p.error == false)
        {
            Debug.Log("Path is assigned");
            path = p;
            currentWaypoint = 0;
        }
    }

 

    private void onDeath()
    {
        Destroy(this.gameObject);
    }

    
}
