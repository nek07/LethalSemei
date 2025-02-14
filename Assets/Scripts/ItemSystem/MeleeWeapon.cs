using System.Collections;
using System.Collections.Generic;
using ItemSystem;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeWeapon : Item
{
    private float timePassed = 0f;
    private float clipLength = 0f;
    private float clipSpeed = 0f;
    private bool attack;
    private bool active = false;
    private KeyCode attackKeyCode = KeyCode.L;
    

    public MeleeWeapon(ItemSO itemSO, GameObject prefab, bool active) : base(itemSO, prefab)
    {
        this.active = active;
    }

    public override void SetActive(bool active)
    {
        base.SetActive(active);
        Debug.Log("Active AXE: " + active);
        Debug.Log("Status: " + gameObject.activeSelf);

        this.active = active;
        if (active)
        {
            characterAnimController.SetTriggers(CharacterAnimController.PlayerTrigger.DrawSword);
        }
        else
        {
            characterAnimController.SetTriggers(CharacterAnimController.PlayerTrigger.SheathSword);
        }
        
    }

    public override void Update()
    {
        base.Update();
        Debug.Log("UPDate update");
        if (!active)
        {
            return;
        }

        if (Input.GetKeyDown(attackKeyCode))
        {
            Debug.Log("ATAKA");
            characterAnimController.SetTriggers(CharacterAnimController.PlayerTrigger.AttackSword);
            attack = true;
        }
    }


    
}
