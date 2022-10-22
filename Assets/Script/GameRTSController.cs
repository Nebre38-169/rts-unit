using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

///<summary>
///<para><c>Class GameRTSController</c> , herits from <c>MonoBehaviour</c></para>
///Handle selection and order for RTS unit.
///Create a pyramid used to detect collision with unit in a 3D world.
///Those unit are then selectionned and order can be given to them.
///</summary>
public class GameRTSController : MonoBehaviour
{
    //Check if you need to see the selection pyramid
    public bool debug = false;
    
    //Those are you use to handle finale position of multiple unit
    //Will be moved in a different script when order are developed
    public int unitByLign = 10;
    public float spaceBetweenLigns = 2f;
    public float spaceBetweenUnits = 2f;
    //Select the panel use for UI representation.
    //It must containt a rectangle highligthing choosen unit
    [SerializeField] public RectTransform selectionPanel;

    //Use to simulate mouse position in the world
    private Plane floor;
    //Store position of the mouse in the world on left click down
    private Vector3 startPosition;
    //Store position of the mouse in the worl on left click up 
    private Vector3 endPosition;
    //Store position of the mouse on the screen
    private Vector3 startScreenPosition;
    //Store the selection pyramid
    private MeshFilter pyramid;
    private MeshCollider selectionCollider;
    //Store selectionned unit
    private List<AllieUnit> selectedUnit;
    

    private void Awake()
    {
        floor = new Plane(Vector3.up, 0);
        
        selectionCollider = GetComponent<MeshCollider>();
        pyramid = GetComponent<MeshFilter>();
        
        //If debug is not choosen, we deactivate the renderer
        MeshRenderer render = GetComponent<MeshRenderer>();
        render.enabled = debug;
        
        selectedUnit = new List<AllieUnit>();
        
        selectionPanel.GetComponent<Image>().enabled = false;
    }

    private void Update()
    {
        //Selection start when the left click is pressed
        if (Input.GetMouseButtonDown(0))
        {
            //We empty the unit list & reset the selection pyramid
            resetSelection();
            resetSelectionPyramid();
            //We get  position of the mouse on the floor and sotres it.
            startPosition = getMousePositionOnFloor();
            //We get position of the mouse on screen and activate UI
            startScreenPosition = Input.mousePosition;
            selectionPanel.GetComponent<Image>().enabled = true;
            positionSelectionPanel(startScreenPosition, startScreenPosition);
        }
        if (Input.GetMouseButton(0))
        {
            //On each iteration, the UI panel is resized using the start position and the current position
            positionSelectionPanel(startScreenPosition, Input.mousePosition);
        }

        //The selection end when the left click is released
        if (Input.GetMouseButtonUp(0))
        {
            endPosition = getMousePositionOnFloor();
            selectionPanel.GetComponent<Image>().enabled = false;
            //We check if the start and end world mouse position are different to avoid an non-volumic mesh
            if (endPosition != startPosition) generateSelectionPyramid(startPosition, endPosition);
            else
            {
                //If the end and the start position are the same, we just check for a Allie Unit and set it selected
                Unit u = isAUnitPointed(false);
                if(u != null) { selectUnit((AllieUnit)u); }
            }
        }

        //If the right click is pressed, a order is given
        if (Input.GetMouseButtonDown(1))
        {
            resetSelectionPyramid();
            /*We look for an enemy unit. If there is, it is set as a target
             * If there is no unit, we give the floor position to go to
             */
            Unit u = isAUnitPointed(true);
            RessourceSource r = isARessourcePointed();
            if(u != null)
            {
                foreach(Unit selectU in selectedUnit)
                {
                    selectU.setTarget(u);
                }
            }
            else if(r != null)
            {
                foreach(Unit selectU in selectedUnit)
                {
                    selectU.setTargetRessource(r,true);
                }
            }
            else
            {
                Vector3 dir = getMousePositionOnFloor();
                calculatePosition(dir);
            }
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other is CapsuleCollider)
        {
            //When the pyramid is generated, collider behave as if unit entered it.
            AllieUnit u = other.GetComponent<AllieUnit>();
            if (u != null) selectUnit(u);
        }
        
    }

    private void OnDrawGizmos()
    {
        if (debug)
        {
            if (startPosition != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(startPosition, 0.1f);
            }
            if (endPosition != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(endPosition, 0.1f);
            }
            if (pyramid != null && pyramid.mesh != null)
            {
                Gizmos.color = Color.black;
                foreach (Vector3 dot in pyramid.mesh.vertices)
                {
                    Vector3 p = this.transform.TransformPoint(dot);
                    Gizmos.DrawSphere(p, 0.1f);
                }
            }
        }
    }

    /// <summary>
    /// <para><c>Function isAUnitPointed</c></para>
    /// Handle finding a unit on the mouse position.
    /// It ignores spherecial collider and only use capsule collider to find unit.
    /// Also, it only give the first found unit as unit should not stack up.
    /// </summary>
    /// <param name="enemy">A boolean used to indicate if we are looking for an Allie or an Enemy</param>
    /// <returns></returns>
    private Unit isAUnitPointed(bool enemy)
    {
        /*To avoid detecting spherical collider and not the pointed unit, we use RaycastAll to see every
        * Collider in the way. We then loop on all them and get the first one that is a capsule collider
        * which carries a AllieUnit script
        */
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000);
        if (hits.Length > 0)
        {
            int i = 0;
            bool found = false;
            while (i < hits.Length && !found)
            {
                RaycastHit hit = hits[i];
                if (hit.collider is CapsuleCollider)
                {
                    if (enemy)
                    {
                        //We are looking for an enemey
                        if (hit.collider.GetComponent<EnemyUnit>() != null)
                        {
                            found = true;
                            return hit.collider.GetComponent<EnemyUnit>();
                        }
                    }
                    else
                    {
                        //We are looking for an allie
                        if (hit.collider.GetComponent<AllieUnit>() != null)
                        {
                            found = true;
                            return hit.collider.GetComponent<AllieUnit>();
                        }
                    }
                }
                i++;
            }
        }
        return null;
    }

    private RessourceSource isARessourcePointed()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit[] hits = Physics.RaycastAll(ray, 1000);
        if (hits.Length > 0)
        {
            int i = 0;
            bool found = false;
            while (i < hits.Length && !found)
            {
                RessourceSource r = hits[i].collider.GetComponent<RessourceSource>();
                if (r)
                {
                    found = true;
                    return r;
                }
                i++;
            }
        }
        return null;
    }

    ///<summary>
    /// <c>Function calculSelectionPyramid</c>.
    /// <para>Handle calculating every vertexs and faces of the selection pyramid 
    /// and create a mesh from them.
    /// The selection pyramid is created using world floor position of the mouse
    /// and the position of the camera.</para>
    /// <para><paramref name="start"/> Position where the left click was pressed.</para>
    /// <para><paramref name="end"/> Position where the left click was released.</para>
    /// <para><returns>Return a Mesh object that should represent a pyramid</returns></para>
    ///</summary>
    private Mesh calculSelectionPyramid(Vector3 start, Vector3 end)
    {
        //Transform mouse position from world to local
        start = this.transform.InverseTransformPoint(start);
        end = this.transform.InverseTransformPoint(end); 

        Mesh mesh = new Mesh();
        //Create the 5 vertexs of the pyramid
        Vector3[] vertices = new Vector3[5]
        {
            new Vector3(start.x,0,start.z),
            new Vector3(end.x,0,start.z),
            new Vector3(end.x,0,end.z),
            new Vector3(start.x,0,end.z),
            Camera.main.transform.position
        };
        mesh.vertices = vertices;

        //Faces are built from triangle so we have 6 triangles from 5 faces
        //As the base is a rectangle and must be generated from 2 triangles.
        //Numbers are index of vertexs in the previous array, in clockwise order
        int[] tris = new int[18]
        {
            2,1,1,// 1
            0,3,2,// 2
            4,2,3,// 6
            4,3,0,// 5
            0,1,4,// 4
            1,2,4 // 3
        };
        mesh.triangles = tris;

        //Use to handle ligthing.
        mesh.RecalculateNormals();
        return mesh;
    }
    
    ///<summary>
    ///<para>Function generateSelectionPyramid
    ///Create the pyramid and gives it to the mesh collider.
    ///Handle debug if necessary.</para>
    ///<para><paramref name="start"/>Position where the left click was pressed</para>
    ///<para><paramref name="end"/>Poisition where the left click was released</para>
    ///</summary>
    private void generateSelectionPyramid(Vector3 start, Vector3 end)
    {
        Mesh m = calculSelectionPyramid(start, end);
        if (debug) pyramid.mesh = m;
        selectionCollider.sharedMesh = m;
    }
    
    ///<summary>
    ///Function resetSelectionPyramid
    ///Reset the mesh collider's mesh
    ///and handle debug reset if necessary
    ///</summary>
    private void resetSelectionPyramid()
    {
        if (debug) pyramid.mesh = null;
        selectionCollider.sharedMesh = null;
    }
    
    ///<summary>
    ///Function selectUnit
    ///Handle unit selection by adding them to the selectedUnit list
    ///and set the unit as selected
    ///</summary>
    private void selectUnit(AllieUnit u)
    {
        if (!selectedUnit.Contains(u))
        {
            u.setSelected(u);
            selectedUnit.Add(u);
        }
    }
    
    ///<summary>
    ///Function resetSelection
    ///Reset the selection list
    ///and set all selected unit as unselected
    ///</summary>
    private void resetSelection()
    {
        foreach(Unit u in selectedUnit)
        {
            u.setSelected(false);
        }
        selectedUnit.Clear();
    }

    ///<summary>
    ///Function getMousePositionOnFloor
    ///Gives the position of the mouse on a infinit
    ///plane location at y=0.
    ///Could be moved in a utility global class
    ///</summary>
    private Vector3 getMousePositionOnFloor()
    {
        //We convert the 2D mouse position by casting a ray to the floor object.
        // -> Could be better to use a terran object.
        float distance;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (floor.Raycast(ray, out distance))
        {
            //On garde en mémoire la position de la sourie dans le monde.
            return ray.GetPoint(distance);
        }
        else
        {
            return new Vector3();
        }
    }
    
    ///<summary>
    ///<para>Function positionSelectionPanel
    ///Handle position calcul for the UI</para>
    ///<para><paramref name="start"/>Position on the screen where the left click was pressed</para>
    ///<para><paramref name="end"/>Position on the screen where the mouse is</para>
    ///</summary>
    private void positionSelectionPanel(Vector3 start, Vector3 end)
    {
        Vector3 middle = (start + end) / 2;
        selectionPanel.position = middle;
        selectionPanel.localScale = new Vector3(
        Mathf.Abs(start.x - end.x),
        Mathf.Abs(start.y - end.y),
        1);
    }
    
    ///<summary>
    ///<para>Function calculatePosition
    ///Calcul of positions for multiple unit movment</para>
    ///<para><paramref name="position"/>Position of the mouse in the world when the left click was pressed</para>
    ///</summary>
    private void calculatePosition(Vector3 position)
    {
        int j = 0; //indice de ligne
        int i = 0; //indice de colonne
        //Debug.Log(selectedUnit.Count);
        foreach(Unit u in selectedUnit)
        {
            Vector3 pos = new Vector3(
                position.x + (i % 10) * spaceBetweenUnits,
                0,
                position.z + j * spaceBetweenLigns);
            u.setDestination(pos);
            i++;
            if (i > unitByLign) { j++; }
        }
    }

}
