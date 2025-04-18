using System.Collections.Generic;

namespace ItemSystem.Market
{
    public interface IMarket
    {
        public void Sell(Item item);
        public void SellItems();
        public bool Buy(Item item);
        public List<Item> GetItems();
    }
}