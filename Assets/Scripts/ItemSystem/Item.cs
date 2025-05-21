﻿using System;
using UnityEngine;
using Random = System.Random;

namespace ItemSystem
{
    public class Item : MonoBehaviour, IPickable
    {
        public ItemSO itemSO;
        public Rigidbody rb;
        public CharacterAnimController characterAnimController;
        public bool isIntreactable = true;
        public bool onTheShowcase = false;

        public int price;
        public int radiation;

        private Random random;
        public Item(ItemSO itemSO, Rigidbody rb, CharacterAnimController characterAnimController, bool isIntreactable)
        {
            this.itemSO = itemSO;
            this.rb = rb;
            this.characterAnimController = characterAnimController;
            this.isIntreactable = isIntreactable;
        }

        private void Start()
        {
            random = new Random();
            price = random.Next(itemSO.minPrice, itemSO.maxPrice);
            radiation = random.Next(itemSO.minRadiation, itemSO.maxRadiation);
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
            isIntreactable = false;
        }

        public virtual void SetActive(bool state)
        {
            
        }
        

        public virtual void OnDropItem()
        {
            if (rb != null)
            {
                rb.isKinematic = false;
            }
            characterAnimController = null;
            isIntreactable = true;
        }

        public virtual void Update(){}
        public string GetTextInteraction()
        {
            return "Press E to pick up " + itemSO.name;
        }

        public bool isInteractable()
        {
            return isIntreactable;
        }

        public void Interact(Camera camera)
        {
        }
    }
}