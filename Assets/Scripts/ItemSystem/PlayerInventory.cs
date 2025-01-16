using System;
using System.Collections;
using System.Collections.Generic;
using ItemSystem;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("General")]
    public List<InventorySlot> inventorySlots;
    public int currentItem = 0;
    private int slotIndex = 0;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Transform itemHolder;
    private GameObject currentItemObject = null; 
    
    [Space(10)]
    [Header("Keys")]
    [SerializeField] private KeyCode throwKey = KeyCode.G;
    [SerializeField] private KeyCode pickUpKey = KeyCode.E;
    
    [Header("PickUp")] 
    [SerializeField] private float pickUpDistance = 1.5f;

   

    private void Update()
    {
        HandleThrowPickInput();
        HandleSelectSlotInput();
    }

    private void HandleSelectSlotInput()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0)
        {
            currentItem = (currentItem + 1) % inventorySlots.Count;
            
        }else if (scroll < 0)
        {
            currentItem = (currentItem - 1 + inventorySlots.Count) % inventorySlots.Count;
        }

        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                currentItem = i;
                
            }
        }
        UpdateSelectedSlot();
        
    }

    private void HandleThrowPickInput()
    {
        if (Input.GetKeyDown(throwKey))
        {
            RemoveItem(currentItem);
        }

        if (Input.GetKeyDown(pickUpKey))
        {
            Debug.Log("Pick Up");
            PickUpItem();
        }
    }

    private void UpdateSelectedSlot()
    {
        if(currentItem < 0 || currentItem >= inventorySlots.Count) return;
        if(currentItem == slotIndex) return; //already active

        if (currentItemObject != null)
        {
            Destroy(currentItemObject);
        }
        
        slotIndex = currentItem;
        InventorySlot slot = inventorySlots[slotIndex];
        Debug.Log(slotIndex);
        if (slot.item == null)
        {
            Debug.Log("Slot is empty");
            return;
        }

        currentItemObject = Instantiate(slot.item.prefab, itemHolder.position, itemHolder.rotation);
        currentItemObject.transform.SetParent(itemHolder);
        currentItemObject.transform.localPosition = slot.item.itemSO.itemPositionOffset;
        
        Debug.Log("Предмет переключен на: " + slot.item.itemSO.itemName);

    }
    private void AddItem(Item item)
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.item == null)
            {
                Debug.Log("Slot is empty and aaaaaaaaa");
                slot.SetItem(item);
                Debug.Log(item.itemSO.itemName + " added to Inventory");
                return;
            }
        }
        Debug.Log("Инвентарь полный!!!!!");
    }

    private void RemoveItem(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < inventorySlots.Count)
        {
            inventorySlots[slotIndex].ClearSlot();
        }
    }

    private void PickUpItem()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickUpDistance))
        {
            if (hit.collider.TryGetComponent(out IPickable pickableItem))
            {
                AddItem(pickableItem.GetItem());
                Debug.Log(pickableItem.GetItem().itemSO.itemName + " test added to Inventory");
                pickableItem.OnPickItem();
                Debug.Log(pickableItem.GetItem().itemSO.itemName + " after onPickItem");
            }
        }
    }
}


public interface IPickable
{
    Item GetItem();
    void OnPickItem();
}
