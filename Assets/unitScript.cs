using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class unitScript : MonoBehaviour
{
    public float speed = 5f;
    public float nextWaypointDistance = 1f;
    private Path path;
    private int currentWaypoint = 0;
    private bool reachedEndOfPath = false;
    //private SpriteRenderer m_SpriteRenderer;

    private Seeker seeker;
    private Rigidbody rb;
    // Start is called before the first frame update
    private void Awake()
    {
        //m_SpriteRenderer = GetComponent<SpriteRenderer>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody>();
        SetSelected(false);
    }

    private void FixedUpdate()
    {
        if (path == null)
            return;
        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            rb.velocity = Vector2.zero;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector3 direction = (path.vectorPath[currentWaypoint] - rb.position).normalized;
        Vector3 force = direction * speed * Time.deltaTime;
        rb.velocity = new Vector3(force.x, 0, force.z);

        float distance = Vector3.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distance < nextWaypointDistance)
        {
            currentWaypoint++;
        }
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    public void generatPath(Vector3 endPosition)
    {
        seeker.StartPath(rb.position, endPosition, OnPathComplete);
    }

    public void SetSelected(bool selected)
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
}
