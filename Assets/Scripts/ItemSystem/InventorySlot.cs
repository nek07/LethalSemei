using System;
using System.Collections;
using System.Collections.Generic;
using ItemSystem;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public Image itemIcon;
    public Text itemName;
    private Sprite itemSprite;
    public Item item;
    

    public void SetItem(Item item1)
    {
        itemSprite = item1.itemSO.itemSprite;
        itemIcon.sprite = itemSprite;
        
        //itemName.text = item.name;
        itemIcon.enabled = itemIcon.sprite != null;
        item = item1;
        Debug.Log(item.itemSO.itemName + " in Invetory slot");
    }

    public void ClearSlot()
    {
        SetItem(null);
    }
    
    
}
