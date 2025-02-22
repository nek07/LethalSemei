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
    private Animator animator;
    private bool active = false;
    private KeyCode attackKeyCode = KeyCode.Mouse0;
    private bool alreadyAttacked;
    

    public MeleeWeapon(ItemSO itemSO, bool active, Rigidbody rb, CharacterAnimController anim, bool isInteractive) : base(itemSO, rb, anim, isInteractive)
    {
        this.active = active;
    }

    public override void SetActive(bool state)
    {
        base.SetActive(active);
        
        Debug.Log("Active AXE: " + active);
        Debug.Log("Status: " + gameObject.activeSelf);

        active = state;
        if (active)
        {
            gameObject.SetActive(true);
            characterAnimController.SetTriggers(CharacterAnimController.PlayerTrigger.DrawSword);
            animator = characterAnimController.GetAnimator();
            
            alreadyAttacked = false;
        }
        else
        {
            characterAnimController.SetTriggers(CharacterAnimController.PlayerTrigger.SheathSword);

        }
        
    }

    public override void Update()
    {
        base.Update();
        Debug.Log("UPDate update " + active);
        if (!active)
        {
            return;
        }

        HandleAttack();
    }

    public override void OnDropItem()
    {
        active = false;
        characterAnimController.SetTriggers(CharacterAnimController.PlayerTrigger.SheathSword);
        base.OnDropItem();
    }

    private void HandleAttack()
    {
        timePassed += Time.deltaTime;
        if ((timePassed >= 1 && Input.GetMouseButtonDown(0)) || (!alreadyAttacked && Input.GetMouseButtonDown(0)))
        {
            Debug.Log("ATAKA");
            characterAnimController.SetTriggers(CharacterAnimController.PlayerTrigger.AttackSword);
            timePassed = 0;
            alreadyAttacked = true;
        }
    }
    


    
}
