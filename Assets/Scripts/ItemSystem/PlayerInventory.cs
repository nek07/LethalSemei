using System;
using System.Collections;
using System.Collections.Generic;
using ItemSystem;
using Unity.VisualScripting;
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
        // Проверяем валидность текущего элемента
        if (currentItem < 0 || currentItem >= inventorySlots.Count)
        {
            Debug.LogWarning("Current item index out of bounds.");
            return;
        }

        // Если слот не изменился, ничего не делаем
        if (currentItem == slotIndex) return;

        // Деактивируем предыдущий активный слот (если есть)
        if (slotIndex >= 0 && slotIndex < inventorySlots.Count)
        {
            InventorySlot previousSlot = inventorySlots[slotIndex];
            if (previousSlot != null && previousSlot.item != null)
            {
                previousSlot.DeactivateSlot();
                Debug.Log("Previous slot deactivated: " + slotIndex);
            }
        }

        // Обновляем текущий индекс
        slotIndex = currentItem;

        // Получаем текущий слот
        InventorySlot slot = inventorySlots[slotIndex];

        // Проверяем, есть ли предмет в слоте
        if (slot == null || slot.item == null)
        {
            Debug.Log("Slot is empty or invalid: " + slotIndex);
            return;
        }

        // Активируем новый слот
        slot.ActivateSlot();
        Debug.Log("Предмет переключен на: " + slot.item.itemSO.itemName);
    }
    private bool AddItem(Item item)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            var slot = inventorySlots[i];
            if (slot.item == null)
            {
                Debug.Log("Slot is empty and aaaaaaaaa");
                GameObject newItem = Instantiate(item.prefab, itemHolder.position, itemHolder.rotation);
                newItem.transform.SetParent(itemHolder);
                newItem.transform.localPosition = item.itemSO.itemPositionOffset;

                if (i != currentItem)
                {
                    newItem.SetActive(false);
                }
                slot.SetSlot(newItem);
                Debug.Log(item.itemSO.itemName + " added to Inventory");
                return true;
            }
        }
        Debug.Log("Инвентарь полный!!!!!");
        return false;
    }

    private void RemoveItem(int slotIndex)
    {
        // Проверяем, что индекс слота находится в допустимых границах
        if (slotIndex < 0 || slotIndex >= inventorySlots.Count)
        {
            Debug.LogWarning("Invalid slot index: " + slotIndex);
            return;
        }

        InventorySlot slot = inventorySlots[slotIndex];

        // Проверяем, есть ли предмет в слоте
        if (slot.item == null)
        {
            Debug.Log("Slot is empty: " + slotIndex);
            return;
        }

        
        // Открепляем объект от родительского контейнера
        GameObject itemGameObject = slot.item.GameObject();
        if (itemGameObject != null)
        {
            itemGameObject.name = slot.item.itemSO.itemName;
            itemGameObject.transform.SetParent(null);
        }
        slot.item.OnDropItem();

        // Очищаем слот
        slot.ClearSlot();

        Debug.Log("Item removed from slot: " + slotIndex);
    }


    private void PickUpItem()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, pickUpDistance))
        {
            if (hit.collider.TryGetComponent(out IPickable pickableItem))
            {
                if(!AddItem(pickableItem.GetItem()))
                    return;
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
