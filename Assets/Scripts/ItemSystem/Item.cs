using System;
using UnityEngine;

namespace ItemSystem
{
    public class Item : MonoBehaviour, IPickable
    {
        public ItemSO itemSO;
        public Rigidbody rb;
        public CharacterAnimController characterAnimController;

        public Item(ItemSO itemSO, Rigidbody rb, CharacterAnimController characterAnimController)
        {
            this.itemSO = itemSO;
            this.rb = rb;
            this.characterAnimController = characterAnimController;
        }

        public void SetCharacterAnimController(CharacterAnimController characterAnimController)
        {
            this.characterAnimController = characterAnimController;
        }
        public Item GetItem()
        {
            return this;
        }

        public void OnPickItem()
        {
            if (rb != null)
            {
                rb.isKinematic = true;
            }
        }

        public virtual void SetActive(bool state)
        {
            
        }
        

        public void OnDropItem()
        {
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            characterAnimController = null;
        }

        public virtual void Update(){}
    }
}