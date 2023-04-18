using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot
{
    public Item item;
    public uint amount;
    public InventorySlot(Item _item, uint _amount)
    {
        item = _item;
        amount = _amount;
    }
    public void AddAmount(uint value)
    {
        amount += value;
    }
}