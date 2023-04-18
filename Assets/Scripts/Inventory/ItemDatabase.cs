using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory/Database")]
public class ItemDatabase : ScriptableObject
{
    public Item[] Items;
    public Item GetItem( string itemId)
    {
        return Items.First(item => item.id == itemId);
    }
}
