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
    public GameObject slotGameObject;
    

    public void SetSlot(GameObject slotGameObject)
    {
        this.slotGameObject = slotGameObject;
        if (slotGameObject != null)
        {
            item = slotGameObject.GetComponent<Item>();
            item.rb.isKinematic = true;
        }
            
        itemSprite = item.itemSO.itemSprite;
        itemIcon.sprite = itemSprite;
        
        //itemName.text = item.name;
        itemIcon.enabled = itemIcon.sprite != null;
        Debug.Log(item.itemSO.itemName + " in Invetory slot");
    }

    public void ActivateSlot()
    {
        if (slotGameObject != null)
        {
            slotGameObject.SetActive(true);
            itemIcon.color = Color.white;
        }
        else
        {
            Debug.LogWarning("slotGameObject не установлен в InventorySlot");
        }
    }

    public void DeactivateSlot()
    {
        if (slotGameObject != null)
        {
            slotGameObject.SetActive(false);
            itemIcon.color = new Color(51, 51, 51, 255);
        }
        else
        {
            Debug.LogWarning("slotGameObject не установлен в InventorySlot");
        }
    }
    public void ClearSlot()
    {
        slotGameObject = null;
        itemSprite = null;
        itemIcon.sprite = itemSprite;
        itemIcon.enabled = itemIcon.sprite != null;
        item = null;
    }
    
    
}
