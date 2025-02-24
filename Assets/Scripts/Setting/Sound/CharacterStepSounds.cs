using UnityEngine;
using Cinemachine;

namespace Setting.Sound
{
    public class CharacterStepSounds : MonoBehaviour
    {
        [SerializeField] private bool useFootsteps = true;

        [Header("Footstep Param")] 
        [SerializeField] private float baseStepSpeed = 0.5f;
        [SerializeField] private float crouchStepMultipler = 1.5f;
        [SerializeField] private float sprintStepMultipler = 0.6f;
        [SerializeField] private AudioSource footStepAudioSource;
        [SerializeField] private AudioClip[] woodClips;
        [SerializeField] private AudioClip[] metalClips;
        [SerializeField] private AudioClip[] grassClips;
        [SerializeField] private AudioClip[] dirtClips;

        private CharacterController characterController;
        private FirstPersonController firstPersonController;
        
 

        private float GetCurrentOffset => firstPersonController.isCrouching ? baseStepSpeed * crouchStepMultipler :
            firstPersonController.IsSprinting ? baseStepSpeed * sprintStepMultipler : baseStepSpeed;

        private Vector3 moveDirection;
        private Vector2 currentInput;
        private float footstepTimer = 0;

        private AudioClip GetRandomClip(AudioClip[] clips)
        {
            if (clips == null || clips.Length == 0) return null;
            return clips[Random.Range(0, clips.Length)];
        }

        public void HandleFootsteps()
        {
            if (!characterController.isGrounded) return;
            if (currentInput == Vector2.zero) return;
            if (footStepAudioSource == null || !footStepAudioSource.enabled) return;

            footstepTimer -= Time.deltaTime;
            if (footstepTimer > 0) return;

            if (Physics.Raycast(firstPersonController.playerCamera.transform.position, Vector3.down, out RaycastHit hit, 1.5f))
            {
                Debug.Log("Ground Tag: " + hit.collider.tag);

                AudioClip clip = null;
                switch (hit.collider.tag)
                {
                    case "Footsteps/WOOD":
                        clip = GetRandomClip(woodClips);
                        break;
                    case "Footsteps/METAL":
                        clip = GetRandomClip(metalClips);
                        break;
                    case "Footsteps/DIRT":
                        clip = GetRandomClip(dirtClips);
                        break;
                    case "Footsteps/GRASS":
                        clip = GetRandomClip(grassClips);
                        break;
                }

                if (clip != null)
                    footStepAudioSource.PlayOneShot(clip);
            }

            footstepTimer = GetCurrentOffset;
        }

        void Start()
        {
            firstPersonController = GetComponent<FirstPersonController>();
            characterController = GetComponent<CharacterController>();
        }

        void Update()
        {
            currentInput = new Vector2(characterController.velocity.x, characterController.velocity.z);
            HandleFootsteps();
        }
    }
    
}
