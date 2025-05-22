using System;
using UnityEngine;

namespace ItemSystem
{
    public class FlashlightItem : Item
    {
        public Light flashlight;
        public KeyCode toggleKey = KeyCode.F;
        [SerializeField] private AudioSource flashlightSound; 

        public FlashlightItem(ItemSO itemSO, Rigidbody rb, CharacterAnimController characterAnimController, bool isIntreactable, Light flashlight, KeyCode toggleKey) : base(itemSO, rb, characterAnimController, isIntreactable)
        {
            this.flashlight = flashlight;
            this.toggleKey = toggleKey;
            
        }

        public override void SetActive(bool state)
        {
            // Этот метод вызывается, когда предмет берут в ру
            isIntreactable = !state;
            if (flashlight != null)
                flashlight.enabled = false;
        }

        public override void Update()
        {
            if (isIntreactable) return;

            if (Input.GetKeyDown(toggleKey))
            {
                if (flashlight != null)
                {
                    flashlightSound.Play();
                    flashlight.enabled = !flashlight.enabled;
                }
            }
        }

        public override void OnDropItem()
        {
            base.OnDropItem();
            isIntreactable = true;
            if (flashlight != null)
            {
                flashlightSound.Play();
                flashlight.enabled = false;
            }
        }
    }
}