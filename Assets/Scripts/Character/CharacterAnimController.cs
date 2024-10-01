using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool isRunning;

    public void SetRunning(bool state)
    {
        animator.SetBool("IsRunning" ,state);
    }
}
