using System.Collections;
using System.Collections.Generic;
using ItemSystem;
using UnityEngine;

public class SimpleItem : Item
{
    public SimpleItem(ItemSO itemSo, Rigidbody rb, CharacterAnimController anim, bool isInteractive) : base(itemSo,rb, anim, isInteractive)
    {
    }

    

    public Item GetItem()
    {
        return this;
    }
}