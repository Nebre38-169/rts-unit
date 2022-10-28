using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New building", menuName = "RTS/Building")]
public class Building : ScriptableObject
{
    public Sprite icon;
    public GameObject prefabs;
    public float constructionDuration;
}
