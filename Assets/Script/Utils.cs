using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Utils
{
    static public Vector3 getMousePositionOnFloor()
    {
        ///<summary>
        ///Function getMousePositionOnFloor
        ///Gives the position of the mouse on a infinit
        ///plane location at y=0.
        ///Could be moved in a utility global class
        ///</summary>
        float distance;
        Plane floor = new Plane(Vector3.up, 0);
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

    static public bool isEnemy(Target source,Target target)
    {
        return source.ally != target.ally;
    }

    static public bool isAlly(Target source,Target target)
    {
        return !isEnemy(source,target);
    }

    ///Returns 'true' if we touched or hovering on Unity UI element.
    public static bool isPointerOverUIElement()
    {
        return isPointerOverUIElement(getEventSystemRaycastResults());
    }
    ///Returns 'true' if we touched or hovering on Unity UI element.
    public static bool isPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == LayerMask.NameToLayer("UI"))
                return true;
        }
        return false;
    }
    ///Gets all event systen raycast results of current mouse or touch position.
    static List<RaycastResult> getEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}
