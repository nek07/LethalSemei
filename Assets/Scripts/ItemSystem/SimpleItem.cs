using System.Collections;
using System.Collections.Generic;
using ItemSystem;
using UnityEngine;

public class SimpleItem : Item
{
    public SimpleItem(ItemSO itemSo, Rigidbody rb, CharacterAnimController anim) : base(itemSo,rb, anim)
    {
    }

    

    public Item GetItem()
    {
        return this;
    }
}
