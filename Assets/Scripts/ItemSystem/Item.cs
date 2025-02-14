using System;
using UnityEngine;

namespace ItemSystem
{
    public class Item : MonoBehaviour, IPickable
    {
        public ItemSO itemSO;
        public GameObject prefab;
        public Rigidbody rb;
        protected CharacterAnimController characterAnimController;

        public Item(ItemSO itemSO, GameObject prefab)
        {
            this.itemSO = itemSO;
            this.prefab = prefab;
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
            if (!state)
            {
                characterAnimController = null;
            }
        }
        

        public void OnDropItem()
        {
            if (rb != null)
            {
                rb.isKinematic = false;
            }
        }

        public virtual void Update(){}
    }
}