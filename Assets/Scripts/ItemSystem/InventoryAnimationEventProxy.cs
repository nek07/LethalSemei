using UnityEngine;

public class InventoryAnimationEventProxy : MonoBehaviour
{
    private PlayerInventory inventory;

    private void Start()
    {
        inventory = GetComponentInParent<PlayerInventory>();
    }

    public void StartDealDamage()
    {
        if (inventory != null)
        {
            inventory.StartDealDamage();
            Debug.Log("StartDealDamage called");
        }
    }
    public void EndDealDamage()
    {
        if (inventory != null)
        {
            inventory.EndDealDamage();
            Debug.Log("EndDealDamage called");
        }
    }

    
}