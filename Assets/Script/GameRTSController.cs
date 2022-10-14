using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRTSController : MonoBehaviour
{
    public bool debug = false;
    public int unitByLign = 10;
    public float spaceBetweenLigns = 2f;
    public float spaceBetweenUnits = 2f;
    [SerializeField] public RectTransform selectionPanel;

    private Plane floor;
    private Vector3 startPosition;
    private Vector3 endPosition;
    private Vector3 startScreenPosition;
    private MeshFilter pyramid;
    private MeshCollider selectionCollider;
    private List<unitScript> selectedUnit;
    

    private void Awake()
    {
        floor = new Plane(Vector3.up, 0);
        selectionCollider = GetComponent<MeshCollider>();
        pyramid = GetComponent<MeshFilter>();
        //On désactive le render si on est pas en débug pour éviter des calculs supplémentaires
        MeshRenderer render = GetComponent<MeshRenderer>();
        render.enabled = debug;
        selectedUnit = new List<unitScript>();
        selectionPanel.GetComponent<Image>().enabled = false;
    }

    private void Update()
    {
        //La sélection commence quand le clic gauche de la sourie est enfoncé.
        if (Input.GetMouseButtonDown(0))
        {
            //On vide la liste des unités sélectionner et on reset la pyramide de sélection
            resetSelection();
            resetSelectionPyramid();
            //On recupére la position de la souris sur le sol
            startPosition = getMousePositionOnFloor();
            startScreenPosition = Input.mousePosition;
            selectionPanel.GetComponent<Image>().enabled = true;
            positionSelectionPanel(startScreenPosition, startScreenPosition);
        }
        if (Input.GetMouseButton(0))
        {
            positionSelectionPanel(startScreenPosition, Input.mousePosition);
        }

        //La sélection prend fin lorsque le clic gauche est relaché
        if (Input.GetMouseButtonUp(0))
        {
            endPosition = getMousePositionOnFloor();
            selectionPanel.GetComponent<Image>().enabled = false;
            // On vérifie si le point de départ et d'arrivée de la sélection ne sont pas les mêmes pour éviter 
            // une pyramide vide
            if (endPosition != startPosition) generateSelectionPyramid(startPosition, endPosition);
            else
            {
                //Si les points sont les mêmes on veut quand même sélectionner l'unité.
                //Cette fois, on teste pour une raycast vers n'importe quel objet et on ne garde l'objet que s'il porte le script d'unité
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

        //Si on appuis sur le clic droit de la souris on donne un ordre 
        if (Input.GetMouseButtonDown(1))
        {
            resetSelectionPyramid();
            Vector3 dir = getMousePositionOnFloor();
            calculatePosition(dir);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Quand la pyramide est généré, les colliders se comportent comme s'ils rentraient dedans
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

    private Mesh calculSelectionPyramid(Vector3 start, Vector3 end)
    {
        //Génére un mesh de pyramide, dont la base est sur le sol et la pointe sur la caméra
        start = this.transform.InverseTransformPoint(start); //Convertis les position global en position local
        end = this.transform.InverseTransformPoint(end); 

        Mesh mesh = new Mesh();
        //Genere les 5 sommets de la pyramide
        Vector3[] vertices = new Vector3[5]
        {
            new Vector3(start.x,0,start.z),
            new Vector3(end.x,0,start.z),
            new Vector3(end.x,0,end.z),
            new Vector3(start.x,0,end.z),
            Camera.main.transform.position
        };
        mesh.vertices = vertices;

        //Genere les triangles composant les faces
        //Les numéros sont les indices des sommets dans le sens horaire face à la caméra
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

        //On recalcul les normales pour pouvoir calculer les lumières
        mesh.RecalculateNormals();
        return mesh;
    }

    private void generateSelectionPyramid(Vector3 start, Vector3 end)
    {
        Mesh m = calculSelectionPyramid(start, end);
        if (debug) pyramid.mesh = m;
        selectionCollider.sharedMesh = m; //On génére un collider avec le même mesh pour détecter la présence d'unité.
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