using System.Collections;
using System.Collections.Generic;
using ItemSystem;
using UnityEngine;

public class SimpleItem : Item
{
    public SimpleItem(ItemSO itemSo, GameObject prefab) : base(itemSo, prefab)
    {
    }

    public void OnPickItem()
    {
        Debug.Log("SimpleItem.OnPickItem");
        //Destroy(gameObject);
        
    }

    public Item GetItem()
    {
        return this;
    }
}
