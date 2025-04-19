using System.Collections;
using Cinemachine;
using Mirror;
using UnityEngine;

public class NetworkFirstPersonController : NetworkBehaviour, IDieable
{
    
    public bool canMove = true;
    public bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    private bool ShouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded;
    private bool ShouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;

    public Vector2 PlayerInput { get; private set; } = Vector2.zero;

    [Header("Options")]
    [SerializeField] private bool canSprint = true;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool canUseHeadBob = true;
    [SerializeField] private bool willSlideOnSlopes = true;
    [SerializeField] private bool useFootsteps = true;

    [Header("Controls")]
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Movement Parameters")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintSpeed = 6.0f;
    [SerializeField] private float crouchSpeed = 1.5f;
    [SerializeField] private float slopeSpeed = 8f;

    [Header("Look Parameters")]
    [SerializeField] public CinemachineVirtualCamera playerCamera;
    [SerializeField, Range(0, 10)] private float lookSpeedX = 2.0f;
    [SerializeField, Range(0, 10)] private float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 180)] private float upperLookLimit = 80.0f;
    [SerializeField, Range(1, 180)] private float lowerLookLimit = 80.0f;
    [SerializeField] private Transform headTarget;

    [Header("Jumping")]
    [SerializeField] private float jumpForce = 8.0f;
    [SerializeField] private float gravity = 23.0f;
    [SerializeField] private float coyoteTime = 0.2f;
    private float coyoteCounter = 0f;

    [Header("Crouch")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 standingCenter = new Vector3(0, 0, 0);
    public bool isCrouching;
    private bool duringCrouchAnimation;

    [Header("Headbob")]
    [SerializeField] private float walkBobSpeed = 14f;
    [SerializeField] private float walkBobAmount = 0.05f;
    [SerializeField] private float sprintBobSpeed = 18f;
    [SerializeField] private float sprintBobAmount = 0.1f;
    [SerializeField] private float crouchBobSpeed = 8f;
    [SerializeField] private float crouchBobAmount = 0.025f;
    private float defaultYPos = 0;
    private float timer;

    [Header("Footsteps")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultipler = 1.5f;
    [SerializeField] private float sprintStepMultipler = 0.6f;
    [SerializeField] private AudioSource runAudioSource = default;

    private float GetCurrentOffset => isCrouching ? baseStepSpeed * crouchStepMultipler :
        IsSprinting ? baseStepSpeed * sprintStepMultipler : baseStepSpeed;

    private Vector3 hitPointNormal;
    private CharacterController characterController;
    private Vector3 moveDirection;
    private Vector2 currentInput;
    private float rotationX = 0;
    private PlayerState playerState;

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
            else return false;
        }
    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerState = GetComponent<PlayerState>();

        if (!isLocalPlayer)
        {
            playerCamera.gameObject.SetActive(false);
            return;
        }

        defaultYPos = playerCamera.transform.localPosition.y;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!isLocalPlayer || !canMove) return;
      

        HandleMovementInput();
        HandleMouseLook();
        if (canJump) HandleJump();
        if (canCrouch) HandleCrouch();
        if (canUseHeadBob) HandleHeadBob();
        ApplyFinalMovements();
        HandleRunningSound();
        UpdateMovementState();
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        transform.position = position;
        Physics.SyncTransforms();
    }

    private void HandleMovementInput()
    {
        float speed = isCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : walkSpeed;
        currentInput = new Vector2(speed * Input.GetAxis("Vertical"), speed * Input.GetAxis("Horizontal"));
        float moveY = moveDirection.y;
        moveDirection = transform.forward * currentInput.x + transform.right * currentInput.y;
        moveDirection.y = moveY;
        PlayerInput = currentInput;
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        headTarget.localPosition = Vector3.Slerp(headTarget.localPosition, new Vector3(0, rotationX / -15, 4f), Time.deltaTime * 50);
        playerCamera.transform.localRotation = Quaternion.Slerp(playerCamera.transform.localRotation, Quaternion.Euler(rotationX, 0, 0), Time.deltaTime * 50);
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

    private void HandleRunningSound()
    {
        if (IsSprinting && characterController.isGrounded)
        {
            if (!runAudioSource.isPlaying)
                runAudioSource.Play();
        }
        else
        {
            if (runAudioSource.isPlaying)
                runAudioSource.Stop();
        }
    }

    private void HandleJump()
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
        if (!characterController.isGrounded) return;

        if (Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : IsSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : IsSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z);
        }
    }

    private void UpdateMovementState()
    {
        PlayerMovementState lateralState = IsMovingLaterally() || PlayerInput != Vector2.zero ? PlayerMovementState.Running : PlayerMovementState.Idling;

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
        }
        else if (IsSprinting)
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

    public void Die()
    {
        canMove = false;
    }
} 