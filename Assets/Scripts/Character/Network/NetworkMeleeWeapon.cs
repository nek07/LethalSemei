/*
using Mirror;
using UnityEngine;
using ItemSystem;

public class NetworkMeleeWeapon : NetworkItem
{
    
    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 1f;

    [Header("Dependencies")]
    [SerializeField] private Animator animator;

    private bool active;
    private float lastAttackTime;

    protected override void Awake()
    {
        base.Awake();
        // Ensure Animator reference is set
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public override void SetActive(bool state)
    {
        base.SetActive(state);
        active = state;

        if (active)
        {
            // Draw weapon animation
            characterAnimController.SetTriggers(CharacterAnimController.PlayerTrigger.DrawSword);
            lastAttackTime = Time.time - attackCooldown;
        }
        else
        {
            // Sheath weapon animation
            characterAnimController.SetTriggers(CharacterAnimController.PlayerTrigger.SheathSword);
        }
    }

    private void Update()
    {
        if (!isLocalPlayer || !active) return;

        // Handle attack input with cooldown
        if (Input.GetMouseButtonDown(0) && Time.time - lastAttackTime >= attackCooldown)
        {
            lastAttackTime = Time.time;
            CmdAttack();
        }
    }

    [Command]
    private void CmdAttack()
    {
        // Server authoritative trigger attack on all clients
        RpcPlayAttack();
        // TODO: server-side hit detection and damage application
    }

    [ClientRpc]
    private void RpcPlayAttack()
    {
        // Play attack animation
        if (characterAnimController != null)
            characterAnimController.SetTriggers(CharacterAnimController.PlayerTrigger.AttackSword);
    }

    // Override drop to include sheath animation
    public  void OnDropItem(Vector3 dropPosition)
    {
        base.OnDropItem(dropPosition);
        active = false;
        // Sheath on drop
        if (characterAnimController != null)
            characterAnimController.SetTriggers(CharacterAnimController.PlayerTrigger.SheathSword);
    }
}
*/
