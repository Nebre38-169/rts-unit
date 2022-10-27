using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
