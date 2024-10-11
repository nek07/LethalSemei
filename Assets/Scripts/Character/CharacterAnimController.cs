using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void SetRunning(bool state)
    {
        animator.SetBool("IsRunning" ,state);
    }
    public void SetWalking(bool state)
    {
        animator.SetBool("IsWalking" ,state);
    }
    public void SetJumping(bool state)
    {
        animator.SetBool("IsJumping" ,state);
    }
    public void SetCrouch(bool state)
    {
        animator.SetBool("IsCrouch" ,state);
    }
    
}
