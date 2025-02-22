using System;
using System.Collections;
using System.Collections.Generic;
using ItemSystem;
using Unity.VisualScripting;
using UnityEditor.Rendering.HighDefinition;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("General")]
    public List<InventorySlot> inventorySlots;
    public int currentItem = 0;
    private int slotIndex = 0;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private CharacterAnimController animController;
    [SerializeField] private Transform itemHolder;
    [SerializeField] private GameObject rightHand;
    
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
        // Проверяем сначала текущий выбранный слот
        var currentSlot = inventorySlots[currentItem];
        if (currentSlot.item == null)
        {
            Debug.Log("Current slot is empty");
            
            return AddItemToSlot(currentItem, item);
        }

        // Если текущий слот занят, проверяем остальные слоты
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (i == currentItem) continue; // Пропускаем уже проверенный слот

            var slot = inventorySlots[i];
            if (slot.item == null)
            {
                Debug.Log("Slot is empty");
                return AddItemToSlot(i, item);
            }
        }

        Debug.Log("Инвентарь полный!!!!!");
        return false;
    }

// Функция для добавления предмета в конкретный слот
    private bool AddItemToSlot(int slotIndex, Item item)
    {
        item.SetCharacterAnimController(animController);
        
        Debug.Log("Character Anim " + item.characterAnimController.name);
        var slot = inventorySlots[slotIndex];
        
        if (item.itemSO.type == ItemType.Melee)
        {
            item.gameObject.transform.SetParent(rightHand.transform);
        }
        else
        {
            item.gameObject.transform.SetParent(itemHolder);
        }
        item.gameObject.transform.localPosition = item.itemSO.itemPositionOffset;
        item.gameObject.transform.localRotation = item.itemSO.itemRotationOffset;

        if (slotIndex != currentItem)
        {
            item.gameObject.SetActive(false);
        }

        slot.SetSlot(item.gameObject, slotIndex == currentItem);
        return true;
    }

    private void RemoveItem(int slotIndex)
    {
        // Проверяем, что индекс слота находится в допустимых границах
        if (slotIndex < 0 || slotIndex >= inventorySlots.Count)
        {
            return;
        }

        InventorySlot slot = inventorySlots[slotIndex];

        // Проверяем, есть ли предмет в слоте
        if (slot.item == null)
        {
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
                pickableItem.OnPickItem();
            }
        }
    }

    public void StartDealDamage()
    {
        try
        {
            if(inventorySlots[currentItem] == null || inventorySlots[currentItem].item.itemSO.type != ItemType.Melee) return;
            Item meleeWeapon = inventorySlots[currentItem].item;

            DamageDealer damageDealer = meleeWeapon.GetComponentInChildren<DamageDealer>();
            damageDealer.StartDealDamage();
            Debug.Log("Starting damage dealer in inventory");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    public void EndDealDamage()
    {
        if(inventorySlots[currentItem] == null || inventorySlots[currentItem].item.itemSO.type != ItemType.Melee) return;
        try
        {
            Item meleeWeapon = inventorySlots[currentItem].item;

            DamageDealer damageDealer = meleeWeapon.GetComponentInChildren<DamageDealer>();
            damageDealer.EndDealDamage();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}


public interface IPickable: IInteractable
{
    Item GetItem();
    void OnPickItem();
}

