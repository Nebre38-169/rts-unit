using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// <c>Class Utils</c>
/// Handles mouse position calculation and mouse over UI detection.
/// Also used for units relation such as enemy and ally
/// </summary>
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

    /// <summary>
    /// <c>Function is Enemy</c>
    /// Indicates whether the target is an enemy of the source
    /// </summary>
    /// <param name="source">A Target aiming at target</param>
    /// <param name="target">The Target aimed at by source</param>
    /// <returns>True if the target is an enemy of source</returns>
    static public bool isEnemy(Target source,Target target)
    {
        return source.ally != target.ally;
    }

    /// <summary>
    /// <c>Function is Enemy</c>
    /// Indicates whether the target is an ally of the source
    /// </summary>
    /// <param name="source">A Target aiming at target</param>
    /// <param name="target">The Target aimed at by source</param>
    /// <returns>True if the target is an ally of source</returns>
    static public bool isAlly(Target source,Target target)
    {
        return !isEnemy(source,target);
    }

    /// <summary>
    /// <c>Function is Pointer Over UI Element</c>
    /// Indicates wheter or not the mouse is over a UI element
    /// </summary>
    /// <returns>True is the mouse is over a UI element</returns>
    public static bool isPointerOverUIElement()
    {
        return isPointerOverUIElement(getEventSystemRaycastResults());
    }

    /// <summary>
    /// <c>Function is Pointer Over UI Element</c>
    /// Indicates whether or not the mouse is over a UI element
    /// using the given Raycast result.
    /// If the raycast result contains a gameObject in the layer mask UI
    /// return true, else return false
    /// </summary>
    /// <param name="eventSystemRaysastResults"></param>
    /// <returns>True is the mouse is over a UI element</returns>
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
    
    /// <summary>
    /// <c>Function get Event System Raycast Results</c>
    /// Get all the raycast for the current mouse position.
    /// Cast ray from the screen and the world (i think ?)
    /// </summary>
    /// <returns></returns>
    static List<RaycastResult> getEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }
}
