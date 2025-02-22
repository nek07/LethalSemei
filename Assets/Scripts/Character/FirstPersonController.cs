using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using Random = UnityEngine.Random;

public class FirstPersonController : MonoBehaviour, IDieable
{
    public bool canMove = true;
    private bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;

    private bool ShouldCrouch =>
        Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;

    public Vector2 PlayerInput { get; private set; } = Vector2.zero;
    private PlayerState playerState;

    [Header("Options")] [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadBob = true;
    [SerializeField] private bool willSlideOnSlopes = true;
    [SerializeField] private bool useFootsteps = true;

    [Header("Controls")] [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Movement Parameters")] [SerializeField]
    private float walkSpeed = 3.0f;

    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float slopeSpeed = 8f;


    [Header("Look Parameters")] [SerializeField]
    public CinemachineVirtualCamera playerCamera;

    [SerializeField, Range(0, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(0, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;
    [SerializeField] private Transform headTarget;

[Header("Jumping Param")] 
    [SerializeField, Range(3f, 20f)] private float jumpForce = 8.0f;
    [SerializeField, Range(1f, 100f)] private float gravity = 23.0f;
    [SerializeField, Range(0f, 0.5f)] private float coyoteTime = 0.2f;
    private float coyoteCounter = 0f;
    
    [Header("Crouch Param")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchAnimation;
    
    [Header("HeadBob Param")] 
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.1f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYPos = 0;
    private float timer;
    
    [Header("Recoil Param")]
    private Vector3 targetRecoil = Vector3.zero;
    private Vector3 currentRecoil = Vector3.zero;


    [Header("Footstep Param")] 
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultipler = 1.5f;
    [SerializeField] private float sprintStepMultipler = 0.6f;
    [SerializeField] private AudioSource footStepAudioSource = default;
    [SerializeField] private AudioClip[] woodClips = default;
    [SerializeField] private AudioClip[] metalClips = default;
    [SerializeField] private AudioClip[] grassClips = default;
    [SerializeField] private AudioClip[] dirtClips = default;

    private float footstepTimer = 0;

    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultipler :
        IsSprinting ? baseStepSpeed * sprintStepMultipler : baseStepSpeed;

    
    
    // SLIDING PARAMS
    private Vector3 hitPointNormal;

    private bool IsSliding
    {
        get
        {
            if (characterController.isGrounded &&
                Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
            {
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > characterController.slopeLimit;
            }
            else
            {
                return false;
            }
        }
    }
    

    
    private CharacterController characterController;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private float rotationX = 0;

    private void Awake()
    {
        //playerCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        defaultYPos = playerCamera.transform.localPosition.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerState = GetComponent<PlayerState>();
    }

    private void Update()
    {
        if (canMove)
        {
            HandleMovementInput();
            HandleMouseLook();
            
            if (canJump)
                HandleJump();
            
            if(canCrouch)
                HandleCrouch();
            
            if(canUseHeadBob)
                HandleHeadBob();

            if (useFootsteps)
                HandleFootsteps();
            
            UpdateMovementState();
            ApplyFinalMovements();
            
           // if (characterController.velocity.y <  -1 && characterController.isGrounded)
             //   moveDirection.y = 0;
        }
    }

    


    private void HandleMovementInput()
    {
        currentInput = new Vector2((isCrouching? crouchSpeed: IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (isCrouching? crouchSpeed: IsSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));
        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) +
                        (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
        PlayerInput = currentInput;
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        headTarget.localPosition = Vector3.Slerp(headTarget.localPosition, new Vector3(0, rotationX / -15, 4f), Time.deltaTime * 50);
        playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.localRotation, Quaternion.Euler(rotationX + currentRecoil.y, 0, 0), Time.deltaTime * 50);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void ApplyFinalMovements()
    {
        if (!characterController.isGrounded)
            moveDirection.y -= gravity * Time.deltaTime;

        if (willSlideOnSlopes && IsSliding)
            moveDirection += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;
        
        characterController.Move(moveDirection * Time.deltaTime);

        
    }

    private void HandleJump()
    {
        {
            if (ShouldJump && coyoteCounter >= 0)
            {
                moveDirection.y = jumpForce;
                coyoteCounter = -1f;
            }
            else if (characterController.isGrounded)
            {
                coyoteCounter = coyoteTime;
            }
            else
            {
                coyoteCounter -= Time.deltaTime;
            }
        }

    }

    private void HandleCrouch()
    {
        if (Input.GetKey(crouchKey) && characterController.isGrounded)
        {
            if (!isCrouching)
                SetCrouchState(true);
        }
        else if (isCrouching || IsSprinting || !characterController.isGrounded)
        {
            SetCrouchState(false);
        }
    }
    private void SetCrouchState(bool crouch)
    {
        if (isCrouching == crouch || duringCrouchAnimation)
            return;

        StartCoroutine(CrouchAnimation(crouch));
    }
    private IEnumerator CrouchAnimation(bool crouch)
    {
        duringCrouchAnimation = true;
        float timeElapsed = 0f;

        float targetHeight = crouch ? crouchHeight : standingHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = crouch ? crouchingCenter : standingCenter;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;
        isCrouching = crouch;
        duringCrouchAnimation = false;
    }

    private void HandleHeadBob()
    {
        if(!characterController.isGrounded) return;

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) *
                (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z);
        }
    }

    private void HandleFootsteps()
    {
        if(!characterController.isGrounded) return;
        if (currentInput == Vector2.zero) return;

        footstepTimer -= Time.deltaTime;

        if (footstepTimer <= 0)
        {
            if (Physics.Raycast(playerCamera.transform.position, Vector3.down, out RaycastHit hit, 3))
            {
                switch (hit.collider.tag)
                {
                    case "Footsteps/WOOD":
                        footStepAudioSource.PlayOneShot(woodClips[Random.Range(0, woodClips.Length - 1)]);
                        break;
                    case "Footsteps/METAL":
                        footStepAudioSource.PlayOneShot(metalClips[Random.Range(0, metalClips.Length - 1)]);
                        break;
                    case "Footsteps/DIRT":
                        footStepAudioSource.PlayOneShot(dirtClips[Random.Range(0, dirtClips.Length - 1)]);
                        break;
                    case "Footsteps/GRASS": footStepAudioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length - 1)]);
                        break;
                    default:
                        break;
                }
            }

            footstepTimer = GetCurrentOffset;
        }
    }

    private void UpdateMovementState()
    {
        PlayerMovementState lateralState = IsMovingLaterally() || (PlayerInput !=
            Vector2.zero) ? PlayerMovementState.Running : PlayerMovementState.Idling;
        if (!characterController.isGrounded && characterController.velocity.y > 0f)
        {
            lateralState = PlayerMovementState.Jumping;
        }
        else if (!characterController.isGrounded && characterController.velocity.y <= 0f)
        {
            lateralState = PlayerMovementState.Falling;
        }
        else if (isCrouching)
        {
            lateralState = PlayerMovementState.Crouching;
        }else if (IsSprinting)
        {
            lateralState = PlayerMovementState.Running;
        }
        playerState.SetState(lateralState);
        
        
    }

    private bool IsMovingLaterally()
    {
        Vector3 lateralVelocity = new Vector3(characterController.velocity.x, 0f, characterController.velocity.z);
        return lateralVelocity.magnitude > 0.01f;
    }
    
    public void ApllyRecoil(GunData gunData)
    {
        float recoilX = Random.Range(-gunData.a_maxRecoil.x, gunData.a_maxRecoil.x) * gunData.a_recoilAmount;
        float recoilY = Random.Range(-gunData.a_maxRecoil.y, gunData.a_maxRecoil.y) * gunData.a_recoilAmount;

        targetRecoil += new Vector3(recoilX, recoilY, 0);

        currentRecoil = Vector3.MoveTowards(currentRecoil, targetRecoil, Time.deltaTime * gunData.a_recoilSpeed);
    }

    public void ResetRecoil(GunData gunData)
    {
        currentRecoil = Vector3.MoveTowards(currentRecoil, Vector3.zero, Time.deltaTime * gunData.a_resetRecoilSpeed);
        targetRecoil = Vector3.MoveTowards(targetRecoil, Vector3.zero, Time.deltaTime * gunData.a_resetRecoilSpeed);

    }

    public void Die()
    {
        canMove = false;
    }
    
}
