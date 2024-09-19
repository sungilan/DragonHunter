using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "New Item/item")]
public class Item : ScriptableObject 
{
    public string sprite_name;
    public string item_name;
    public string item_type;
    public string stat;
    public int cost;
    public int value;
}