using UnityEngine;
using Mirror;

public class NetworkPersonAnimationController : NetworkBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private float locomotionBlendSpeed = 1f;

    private FirstPersonController firstPersonController;
    private PlayerState playerState;
    private CharacterController characterController;

    private static int inputXhash = Animator.StringToHash("inputX");
    private static int inputYhash = Animator.StringToHash("inputY");
    private static int isCrouchHash = Animator.StringToHash("IsCrouch");
    private static int isFallingHash = Animator.StringToHash("IsFalling");
    private static int isJumpingHash = Animator.StringToHash("IsJumping");
    private static int isRunningHash = Animator.StringToHash("IsRunning");

    private Vector3 currentBlendInput = Vector3.zero;

    private void Awake()
    {
        firstPersonController = GetComponent<FirstPersonController>();
        playerState = GetComponent<PlayerState>();
        characterController = GetComponent<CharacterController>();
    }

    private float remoteInputX, remoteInputY;

    private void Update()
    {
        if (isLocalPlayer)
        {
            UpdateAnimationState(); // как раньше
        }
        else
        {
            // Плавно интерполируем полученные параметры от NetworkAnimator
            float targetX = animator.GetFloat("inputX");
            float targetY = animator.GetFloat("inputY");

            remoteInputX = Mathf.Lerp(remoteInputX, targetX, Time.deltaTime * 10f);
            remoteInputY = Mathf.Lerp(remoteInputY, targetY, Time.deltaTime * 10f);

            animator.SetFloat("inputX", remoteInputX);
            animator.SetFloat("inputY", remoteInputY);
        }
    }

    private void UpdateAnimationState()
    {
        bool isCrouching = playerState.CurrentPlayerMovementState == PlayerMovementState.Crouching;
        bool isRunning = playerState.CurrentPlayerMovementState == PlayerMovementState.Running;
        bool isJumping = playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping;
        bool isFalling = playerState.CurrentPlayerMovementState == PlayerMovementState.Falling;

        Vector2 inputTarget = firstPersonController.PlayerInput;
        currentBlendInput = Vector3.Lerp(currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);

        animator.SetBool(isCrouchHash, isCrouching);
        animator.SetBool(isFallingHash, isFalling);
        animator.SetBool(isRunningHash, isRunning);
        animator.SetBool(isJumpingHash, isJumping);

        animator.SetFloat(inputXhash, currentBlendInput.x);
        animator.SetFloat(inputYhash, currentBlendInput.y);
    }

    public void SetTriggers(PlayerTrigger trigger)
    {
        if (!isLocalPlayer) return;

        animator.SetTrigger(trigger.ToString());
    }

    public Animator GetAnimator()
    {
        return animator;
    }

    public enum PlayerTrigger
    {
        DrawSword,
        SheathSword,
        AttackSword
    }
}
