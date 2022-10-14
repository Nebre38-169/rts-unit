using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private List<unitScript> selectedUnit;
    

    private void Awake()
    {
        floor = new Plane(Vector3.up, 0);
        
        selectionCollider = GetComponent<MeshCollider>();
        pyramid = GetComponent<MeshFilter>();
        
        //If debug is not choosen, we deactivate the renderer
        MeshRenderer render = GetComponent<MeshRenderer>();
        render.enabled = debug;
        
        selectedUnit = new List<unitScript>();
        
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
                //If the start and end position are the same, we use a raycast to detect if the mouse was over a unit
                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out hit, 1000))
                {
                    if (hit.collider != null && hit.collider.GetComponent<unitScript>() != null)
                    {
                        unitScript u = hit.collider.GetComponent<unitScript>();
                        selectUnit(u);
                    }
                }
            }
        }

        //If the right click is pressed, a move order is given
        if (Input.GetMouseButtonDown(1))
        {
            resetSelectionPyramid();
            Vector3 dir = getMousePositionOnFloor();
            calculatePosition(dir);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //When the pyramid is generated, collider behave as if unit entered it.
        unitScript u = other.GetComponent<unitScript>();
        if (u != null) selectUnit(u);
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

    ///<summary>
    /// Function calculSelectioPyramid.
    /// Handle calculating every vertexs and faces of the selection pyramid 
    /// and create a mesh from them.
    /// The selection pyramid is created using world floor position of the mouse
    /// and the position of the camera.
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

    private void generateSelectionPyramid(Vector3 start, Vector3 end)
    {
        Mesh m = calculSelectionPyramid(start, end);
        if (debug) pyramid.mesh = m;
        selectionCollider.sharedMesh = m;
    }

    private void resetSelectionPyramid()
    {
        if (debug) pyramid.mesh = null;
        selectionCollider.sharedMesh = null;
    }

    private void selectUnit(unitScript u)
    {
        u.SetSelected(u);
        selectedUnit.Add(u);
    }

    private void resetSelection()
    {
        foreach(unitScript u in selectedUnit)
        {
            u.SetSelected(false);
        }
        selectedUnit.Clear();
    }

    private Vector3 getMousePositionOnFloor()
    {
        //On convertis la position de la sourie vers une position sur un plan fixe, infine et placer sur 0.
        // -> Une évolution pourrait être d'utiliser un terrain.
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

    private void positionSelectionPanel(Vector3 start, Vector3 end)
    {
        Vector3 middle = (start + end) / 2;
        selectionPanel.position = middle;
        selectionPanel.localScale = new Vector3(
        Mathf.Abs(start.x - end.x),
        Mathf.Abs(start.y - end.y),
        1);
    }

    private void calculatePosition(Vector3 position)
    {
        int j = 0; //indice de ligne
        int i = 0; //indice de colonne
        foreach(unitScript u in selectedUnit)
        {
            Vector3 pos = new Vector3(
                position.x + (i % 10) * spaceBetweenUnits,
                0,
                position.z + j * spaceBetweenLigns);
            u.generatPath(pos);
            i++;
            if (i > unitByLign) { j++; }
        }
    }

}
