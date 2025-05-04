using System.Collections.Generic;

namespace ItemSystem.Market
{
    public interface IMarket
    {
        public void Sell(Item item);
        public void SellItems();
        public bool Buy(string itemName);
        public List<Item> GetItems();
        public void OpenShop();
    }
}