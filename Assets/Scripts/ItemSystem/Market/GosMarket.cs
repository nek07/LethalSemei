using System.Collections.Generic;
using UnityEngine;

namespace ItemSystem.Market
{
    /*
     * GosMarket features:
     * Limited items to buy, but with infinity amount
     * Buy by money, Sell by radiation conficent
     */
    public class GosMarket: MonoBehaviour, IMarket
    {
        [SerializeField] private List<Item> items;
        [SerializeField] private Transform spawnPoint;
        [SerializeField] private float exchangeRate = 1;
        
        private List<Item> sellItems = new List<Item>();


        public bool Buy(Item item)
        {
            if (!items.Contains(item))
                return false;
            
            if (!TeamManager.Instance.TrySpendMoney(item.price))
                return false;
            
            Instantiate(item, spawnPoint.position, Quaternion.identity);
            return true;
        }

        public void Sell(Item item)
        {
            TeamManager.Instance.AddMoney((item.radiation * exchangeRate));
            Destroy(item);
        }
        
        public void SellItems()
        {
            int total = 0;
            for (int i = 0; i < sellItems.Count; i++)
            {
                total += sellItems[i].price;
                Destroy(items[i]);
            }
            
            TeamManager.Instance.AddMoney(total);
            sellItems.Clear();
        }
        

        public List<Item> GetItems()
        {
            return items;
        }
        
        
        private void OnTriggerEnter(Collider other)
        {
            other.TryGetComponent<Item>(out Item item);
            if (item != null)
            {
                sellItems.Add(item);
            }
        }
    }
}