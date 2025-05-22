using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ItemSystem.Market
{
    public class DarkMarket: MonoBehaviour, IMarket
    {
        [SerializeField] private List<Item> items;
        [SerializeField] private List<Transform> itemsPlaces;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private ShopUI shopUI;

        [SerializeField] private List<Item> sellItems;

        private void Awake()
        {
            sellItems = new List<Item>();
            for (int i = 0; i < itemsPlaces.Count; i++)
            {
                if (i >= items.Count)
                    break;
                Instantiate(items[i], itemsPlaces[i].position, Quaternion.identity).GetComponent<Item>();
            }
            shopUI.CreateItemUI(items);
        }

        public void Sell(Item item)
        {
            TeamManager.Instance.AddMoney((item.price));
            Destroy(item);
        }

        public void SellItems()
        {
            Debug.Log("Try sell items");
            int total = 0;
            for (int i = 0; i < sellItems.Count; i++)
            {
                total += sellItems[i].price;
                Destroy(sellItems[i].gameObject);
            }
            
            TeamManager.Instance.AddMoney(total);
            sellItems.Clear();
        }

        public bool Buy(string itemName)
        {
            // Ищем предмет по имени в списке items
            Item item = items.FirstOrDefault(i => i.itemSO.itemName == itemName);
    
            // Если не найден — выходим
            if (item == null)
                return false;

            if (!TeamManager.Instance.TrySpendMoney(item.itemSO.maxPrice))
                return false;
            
            Instantiate(item, spawnPoint.position, Quaternion.identity);

            items.Remove(item);
            /*Destroy(item);*/

            return true;
        }
        

        public List<Item> GetItems()
        {
            return items;
        }

        public void OpenShop()
        {
            shopUI.ShowShop();
        }
        
        
        private void OnTriggerEnter(Collider other)
        {
            other.TryGetComponent<Item>(out Item item);
            if (item != null)
            {
                sellItems.Add(item);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            other.TryGetComponent<Item>(out Item item);
            if (item != null)
            {
                sellItems.Remove(item);
            }
        }
    }
}