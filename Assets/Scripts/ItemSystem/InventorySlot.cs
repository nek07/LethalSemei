using ItemSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField]public Image itemIcon;
    [SerializeField]private TextMeshProUGUI itemName;
    [SerializeField]private Image slotBorder;
    private Sprite itemSprite;
    private TwoBoneIKConstraint rightHandRig; 
    
    public Item item;
    public GameObject slotGameObject;

    public void SetSlot(GameObject slotGameObject, bool isActive, TwoBoneIKConstraint rightHandRig)
    {
        this.slotGameObject = slotGameObject;
        if (slotGameObject != null)
        {
            item = slotGameObject.GetComponent<Item>();
            item.rb.isKinematic = true;
        }
        this.rightHandRig = rightHandRig;

        itemSprite = item.itemSO.itemSprite;
        itemIcon.sprite = itemSprite;
        itemIcon.enabled = true;
        
        itemIcon.color = Color.white;   // ✅ Иконка предмет
        if (isActive)
        {
            ActivateSlot();
        }
        
        Debug.Log(item.itemSO.itemName + " in Invetory slot");
    }

    public void ActivateSlot()
    {  
        itemName.text = item.itemSO.itemName;
        
        slotBorder.color = Color.white;
        if (slotGameObject != null)
        {
            slotGameObject.SetActive(true);
            item.SetActive(true);
            if (rightHandRig != null && item.itemSO.type != ItemType.Melee)
            {
                rightHandRig.weight = 1f;
            }
        }
        else
        {
            Debug.LogWarning("slotGameObject не установлен в InventorySlot");
        }
    }

    public void DeactivateSlot()
    {
        itemName.text = "";
        item.SetActive(false);
        if (slotGameObject != null)
        {
            slotGameObject.SetActive(false);
            slotBorder.color = Color.black;
            itemIcon.color = new Color(51, 51, 51, 255);
            if (rightHandRig != null)
            {
                rightHandRig.weight = 0f;
            }
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
        itemIcon.enabled = false;
        itemName.text = "";
        slotBorder.color = Color.black;
        item = null;
        if (rightHandRig != null)
        {
            rightHandRig.weight = 0f;
        }
    }
    
    
}