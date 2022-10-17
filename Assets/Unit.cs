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
    public float nextWaypointDistance = 1f;

    //Store path and state of the unit on the path
    private Path path;
    private int currentWaypoint = 0;
    //Used to generate the path
    private Seeker seeker;
    //Used to handle movement
    private Rigidbody rb;
    //Could be used to render a selected state
    //private SpriteRenderer m_SpriteRenderer;


    private void Awake()
    {
        //m_SpriteRenderer = GetComponent<SpriteRenderer>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody>();
        setSelected(false);
    }

    private void FixedUpdate()
    {
        //If there is no path, do nothing
        if (path == null)
            return;
        //If there is a path and our current waypoint is the last one, we stop
        if (currentWaypoint >= path.vectorPath.Count)
        {
            rb.velocity = Vector2.zero;
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

    /// <summary>
    /// <c>Function generatePath</c>
    /// Create a path to the endPosition using the Seeker.
    /// </summary>
    /// <param name="endPosition">Vector3 the final position the unit must reach</param>
    public void generatePath(Vector3 endPosition)
    {
        seeker.StartPath(rb.position, endPosition, onPathComplete);
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
    /// <para>Function onPathComplete</para>
    /// This function is called when the Seeker has finish calculting the path.
    /// It stores the path and reset de waypoint counter
    /// </summary>
    /// <param name="p">Path given by the Seeker</param>
    private void onPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }
}
