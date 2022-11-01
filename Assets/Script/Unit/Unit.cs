using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public enum Order
{
    IDLE,
    MOVE,
    ATTACK,
    FORCEATTACK,
    HARVEST,
    BRING,
    BUILD
}

/// <summary>
/// <para><c>Class Unit</c>, herits from <c>MonoBehaviour</c></para>
/// Handle order execution for move, attack and idle.
/// Handle movement and fight, handle following a target and loosing the target
/// </summary>
public class Unit : Target
{
    public float speed = 5f;
    //This argument is use to smooth the path
    public float nextWaypointDistance = 0.2f;
    //Variable used for combat
    
    public float damage = 5f;
    //Variable used for target selection
    public float searchRange = 10f; //Within this range, an enemy will be picked as a target
    public float attackRange = 2f; //Within this range, an enemy will take damage
    public float lostRange = 20f; //Outside this range, a target will be lost and focused will be lost
    public float coolDownDuration = 10f; //Time between two attacks
    public float harvestRange = 3f;
    public float harvestCoolDown = 5f;
    public float maxLoad = 10f;
    public int unloadPacket = 5; //Define the quantity of ressource the unit can unload

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
    protected Target target;
    protected Order currentOrder;
    private int frameCounter;
    private SphereCollider rangeCollider;
    private TargetTrigger targetDetector;
    //Used to store opponent (unit that store this instance as a target)

    //Used for ressource harvesting
    private RessourceTrigger ressourceDetector; //Must be a child gameObject, used to detect if source and depot are in range
    private RessourceSource targetSource; //Store the selected source
    private Ressource targetRessource; //Store the selected ressource (given by the source)
    private Depot targetDepot; //Store the place where ressource can be offloaded

    private Builder targetBuild;
    
    private float currentLoad = 0f; //Indicates the current load


    new protected void Awake()
    {
        base.Awake();
        //m_SpriteRenderer = GetComponent<SpriteRenderer>();
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody>();
        setSelected(false);
        currentLife = maxLife;
        currentOrder = Order.IDLE;
        //We set the sphere collider to the search range to lookout for enemy unit
        rangeCollider = GetComponent<SphereCollider>();
        rangeCollider.radius = searchRange;
        ressourceDetector = gameObject.GetComponentInChildren<RessourceTrigger>();
        if(ressourceDetector == null)
        {
            throw new System.Exception("Missing ressource detector on unit " + gameObject.name);
        }
        targetDetector = GetComponentInChildren<TargetTrigger>();
        if(targetDetector == null) { throw new System.Exception("Missing target detector on unit " + gameObject.name); }
    }

    private void Start()
    {
        targetDetector.setCollider(attackRange);
    }

    protected void FixedUpdate()
    {
        //If the unit is idle but carries a load, goes to bring it back
        if(currentOrder == Order.IDLE && currentLoad > 0)
        {
            currentOrder = Order.BRING;
        }
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
                    debugMessage("Target in range");
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
        //If the order is move, we just move
        if(currentOrder == Order.MOVE)
        {
            moveAlongPath();
        }
        if(currentOrder == Order.HARVEST)
        {
            if (isRessourceInRange())
            {
                //If the ressource is in range, the unit harvest it every cool down
                onPathComplete();
                if(frameCounter > harvestCoolDown * 60)
                {
                    harvest(targetSource);
                }
                else
                {
                    frameCounter++;
                }
            }
            else
            {
                if (path == null) { generatePath(targetSource.transform.position); }
                moveAlongPath();
            }
        }
        if(currentOrder == Order.BRING)
        {
            if (targetDepot == null)
            {
                debugMessage("Looking for a depot");
                targetDepot = getClosestDepot();
            }
            else
            {
                if (isDepotInRange())
                {
                    onPathComplete();
                    if (frameCounter > harvestCoolDown * 60)
                    {
                        unload(targetDepot);
                    }
                    else
                    {
                        frameCounter++;
                    }
                }
                else
                {
                    if (path == null) { generatePath(targetDepot.transform.position); }
                    moveAlongPath();
                }
            }
        }
        if (currentOrder == Order.BUILD)
        {
            if (isBuildInRange())
            {
                onPathComplete();
            }
            else
            {
                if(path == null) { generatePath(targetBuild.transform.position); }
                moveAlongPath();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Target u = other.GetComponent<Target>();
        if (u != null && isEnemy(u) && target == null)
        {
            debugMessage("Found target " + u.gameObject.name);
            setTarget(u);
            if (currentOrder != Order.MOVE)
            {
                currentOrder = Order.ATTACK;
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
    /// <c>Function setSelected</c>
    /// Handle animation selection (when it is developt)
    /// </summary>
    /// <param name="selected">Boolean that indicates wether or not the unit is selected</param>
    public void setSelected(bool selected)
    {
        if (selected)
        {
            if (debug) { Debug.Log(gameObject.name + " is selected"); }
            //m_SpriteRenderer.color = Color.blue;
        }
        else
        {
            if (debug) { Debug.Log(gameObject.name + " is unselected"); }
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
    public void setTarget(Target u)
    {
        currentOrder = Order.FORCEATTACK;
        target = u;
        target.addOpponent(this);
    }

    /// <summary>
    /// <para><c>Function unSetTarget</c></para>
    /// Used to reset the target selection and order to idle
    /// </summary>
    public void unSetTarget()
    {
        target = null;
        currentOrder = Order.IDLE;
    }

    /// <summary>
    /// <para><c>Function set Target Ressource</c></para>
    /// <para>Set the specified ressource source as the target for harvest,
    /// The unit must not carry ressource or must already carry the same ressource as the source</para>
    /// </summary>
    /// <param name="r">The RessourceSource that will be the target</param>
    /// <param name="order">Used to change the order of the unit or not</param>
    /// <returns>True if the source was set as target, false otherwise</returns>
    public bool setTargetRessource(RessourceSource r, bool order)
    {
        if(targetRessource == null)
        {
            targetSource = r;
            targetRessource = r.ressource;
            r.addHarvester(this);
            if (order) { currentOrder = Order.HARVEST; }
            return true;
        }
        else
        {
            if(targetRessource == r.ressource)
            {
                targetSource = r;
                r.addHarvester(this);
                if (order) { currentOrder = Order.HARVEST; }
                return true;
            }
            else if(currentLoad >0 )
            {
                return false;
            }
            else
            {
                targetRessource = r.ressource;
                targetSource = r;
                r.addHarvester(this);
                if (order) { currentOrder = Order.HARVEST; }
                return true;
            }
        }
    }

    public bool setBuildingTarget(Builder d)
    {
        targetBuild = d;
        targetBuild.addBuilder(this);
        currentOrder = Order.BUILD;
        return true;
    }

    public void unsetBuildingTarger()
    {
        targetBuild = null;
        currentOrder = Order.IDLE;
    }

    /// <summary>
    /// <para><c>Function onPathComplete</c></para>
    /// Used when the destionation is reached or the unit is in attack range of the target.
    /// Reset the velocity and path, and if the order was to move, the unit goes to idle.
    /// </summary>
    public void onPathComplete()
    {
        //Debug.Log("We reached end of path");
        rb.velocity = Vector3.zero;
        path = null;
        currentWaypoint = 0;
        if(currentOrder == Order.MOVE) { currentOrder = Order.IDLE; }
    }

    /// <summary>
    /// <para><c>Function on Ressource Empty</c></para>
    /// <para>Triggered by the source when it is empty,
    /// The given remplacement could be null, if so the unit goes into idle</para>
    /// </summary>
    /// <param name="remplacement">The replacement for the previous source</param>
    public void onRessourceEmpty(RessourceSource remplacement)
    {
        if(remplacement != null)
        {
            debugMessage("Remplacement set");
            setTargetRessource(remplacement, false);
        }
        else
        {
            debugMessage("No remplacement was found, bring the remaining load to depot");
            targetSource = null;
            debugMessage("The current load is " + currentLoad);
            if(currentLoad > 0) { currentOrder = Order.BRING; }
            else { currentOrder = Order.IDLE; }
        }
    }

    /// <summary>
    /// <para><c>Function moveAlongPath</c></para>
    /// Handle unit's movement along a precalculated path
    /// If the unit has no path, do nothing
    /// </summary>
    private void moveAlongPath()
    {
        if (path != null)
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

    /// <summary>
    /// <para><c>Function isTargetInRange</c></para>
    /// Indicates wether the target is in attack range or not
    /// </summary>
    /// <returns>True if the target is in range, False other wise or if there is no target</returns>
    private bool isTargetInRange()
    {
        if (target != null)
        {
            return targetDetector.isTargetInRange(target);
        }
        return false;
    }

    /// <summary>
    /// <para><c>Function is Ressource In Range</c></para>
    /// <para>Ask the Ressource Detector if the source is in range</para>
    /// </summary>
    /// <returns>True if the source is in range, false otherwise</returns>
    private bool isRessourceInRange()
    {
        if(targetSource != null)
        {
            return ressourceDetector.isTargetRessourceInRange(targetSource);
        }
        return false;
    }

    /// <summary>
    /// <para><c>Function is Depot In Range</c></para>
    /// <para>Ask the Ressource Detector if the depot is in range</para>
    /// </summary>
    /// <returns>True if the depot is in range, false otherwise</returns>
    private bool isDepotInRange()
    {
        if (targetDepot != null)
        {
            return ressourceDetector.isTargetDepotInRange(targetDepot);
        }
        return false;
    }

    private bool isBuildInRange()
    {
        if(targetBuild != null)
        {
            return ressourceDetector.isBuildInRange(targetBuild);
        }
        return false;
    }

    /// <summary>
    /// <para><c>Function isTargetLost</c></para>
    /// Indicates wether the target is further than the lost range
    /// </summary>
    /// <returns>True if the target is lost, false otherwise and if there is no target</returns>
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

    /// <summary>
    /// <para><c>Function isTargetAtEndOfPath</c></para>
    /// Check if the target is still at the end of the path
    /// </summary>
    /// <returns>True if the target is within a cirlce of diameter 2 from the end of the path,
    /// false otherwise or if there is no path</returns>
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

    /// <summary>
    /// <para><c>Function targetVerification</c></para>
    /// Check if the target still exist
    /// (Not sure it is usefull)
    /// </summary>
    /// <returns>True if the target still exist, false and unSetTarget otherwise</returns>
    private bool targetVerification()
    {
        if(target.gameObject == null) 
        {
            unSetTarget();
            return false;
        }
        return true;
    }

    /// <summary>
    /// <para><c>Function dealDamage</c></para>
    /// Handle dealing damage to a unit.
    /// </summary>
    /// <param name="u">The target of the attack</param>
    private void dealDamage(Target u)
    {
        u.onTakeDamage(damage);
        frameCounter = 0;
    }

    /// <summary>
    /// <para><c>Function estimate Quantity</c></para>
    /// <para>Calcul the integer quantity of ressource 
    /// the unit can carry</para>
    /// </summary>
    /// <returns>Difference between the current and max load,
    /// divided by the weight of the ressource, rounded to int</returns>
    private int estimateQuantity()
    {
        float diff = maxLoad - currentLoad;
        return Mathf.RoundToInt(diff / targetRessource.weight);
    }

    /// <summary>
    /// <para><c>Function Harvest</c></para>
    /// <para>Handles the logic of harvesting when the source is in range.
    /// Get the estimated quantity (if 0 goes into bring mode),
    /// then harvest and add the given quantity to its load.
    /// If the load is over or egal to the max load, goes into bring mode</para>
    /// </summary>
    /// <param name="r">The source where the ressource is harvested</param>
    private void harvest(RessourceSource r)
    {
        int estimatedQuantity = estimateQuantity();
        if(estimatedQuantity == 0)
        {
            currentOrder = Order.BRING;
            return;
        }
        int quantity = r.onHarvest(this, estimatedQuantity);
        currentLoad += quantity*r.ressource.weight;
        frameCounter = 0;
        if(currentLoad >= maxLoad)
        {
            currentOrder = Order.BRING;
        }
    }

    /// <summary>
    /// <para><c>Function unLoad</c></para>
    /// <para>Handles the logic of unloading into a depot.
    /// Calcul the packet that can be given, either the full packet or what is remaining.
    /// If there is no more ressource to unload, goes into harvest if there is a source,
    /// or goes into idle if there is none.</para>
    /// </summary>
    /// <param name="d"></param>
    private void unload(Depot d)
    {
        if(currentLoad >= unloadPacket * targetRessource.weight)
        {
            currentLoad -= unloadPacket * targetRessource.weight;
            d.onUnLoad(targetRessource, unloadPacket);
        }
        else
        {
            int quantity = Mathf.RoundToInt(currentLoad / targetRessource.weight);
            currentLoad -= quantity * targetRessource.weight;
            d.onUnLoad(targetRessource, quantity);
        }
        
        if (currentLoad <= 0)
        {
            currentLoad = 0;
            if (targetSource != null)
            {
                
                currentOrder = Order.HARVEST;
            }
            else
            {
                currentOrder = Order.IDLE;
            }
        }
        frameCounter = 0;
    }

    /// <summary>
    /// <para><c>Function get Closest Depot</c></para>
    /// <para>Find the closest depot from the unit.
    /// Find all depot and filter depot that can store the ressource,
    /// then find the closest one</para>
    /// </summary>
    /// <returns>The closest depot that can store the ressource</returns>
    private Depot getClosestDepot()
    {
        Depot[] depotArray = GameObject.FindObjectsOfType<Depot>();
        List<Depot> depotList = new List<Depot>();
        for(int i = 0; i < depotArray.Length; i++)
        {
            if (depotArray[i].isRessourceUnloadable(targetRessource)) { depotList.Add(depotArray[i]); }
        }
        if(depotList.Count <= 0)
        {
            return null;
        }
        else if(depotList.Count == 1)
        {
            return depotList[0];
        }
        else
        {
            float closesDistance = Mathf.Infinity;
            Depot candidate = depotList[0];
            foreach(Depot depot in depotList)
            {
                float dist = Mathf.Abs(Vector3.Distance(transform.position, depot.transform.position));
                if (dist < closesDistance)
                {
                    closesDistance = dist;
                    candidate = depot;
                }
            }
            return candidate;
        }
    }

    private bool isEnemy(Target u)
    {
        return ally != u.ally;
    }
    
    private bool isAlly(Target u)
    {
        return !isEnemy(u);
    }
}
