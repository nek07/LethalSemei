
using UnityEngine;

public class CharacterAnimController : MonoBehaviour
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

    private void Update()
    {
        UpdateAnimationState();
    }

    private void UpdateAnimationState()
    {
        bool isCrouching = playerState.CurrentPlayerMovementState == PlayerMovementState.Crouching;
        bool isRunning = playerState.CurrentPlayerMovementState == PlayerMovementState.Running;
        bool isJumping = playerState.CurrentPlayerMovementState == PlayerMovementState.Jumping;
        bool isFalling = playerState.CurrentPlayerMovementState == PlayerMovementState.Falling;
        bool isGrounded = characterController.isGrounded;
        bool isSprinting = playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
        
        Vector2 inputTarget = firstPersonController.PlayerInput;
        currentBlendInput = Vector3.Lerp(currentBlendInput, inputTarget, locomotionBlendSpeed * Time.deltaTime);
        
        animator.SetBool(isCrouchHash, isCrouching);
        animator.SetBool(isFallingHash, isFalling);
        animator.SetBool(isRunningHash, isRunning);
        //animator.SetBool(isRunningHash, isSprinting);
        //animator.SetBool(, isGrounded);
        animator.SetBool(isJumpingHash, isJumping);
        animator.SetFloat(inputXhash, currentBlendInput.x);
        animator.SetFloat(inputYhash, currentBlendInput.y);
        
    }
    
}
