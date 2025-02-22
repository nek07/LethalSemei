using ItemSystem;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class ItemRigController : MonoBehaviour
{
    public Transform rightHandTarget;
    public Transform leftHandTarget;
    public Rig weaponRig;
    
    void Start()
    {
        // Убедимся, что Rig включен
        weaponRig.weight = 1f;
    }

    public void EquipItem(Item item)
    {
        // Привязываем оружие к руке
        item.gameObject.transform.SetParent(rightHandTarget);
        item.gameObject.transform.localPosition = Vector3.zero;
        item.gameObject.transform.localRotation = Quaternion.identity;

        // Устанавливаем цели IK для рук
        rightHandTarget.position = item.itemSO.rightHandPosition;
        rightHandTarget.rotation = item.itemSO.rightHandRotation;

        //leftHandTarget.position = item.gameObject.transform.Find("LeftHandHold").position;
        //leftHandTarget.rotation = item.gameObject.transform.Find("LeftHandHold").rotation;
    }
}