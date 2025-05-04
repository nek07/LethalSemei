using System.ComponentModel;
using UnityEngine;

namespace ItemSystem.Market
{
    public class MarketButton: MonoBehaviour, IInteractable
    {
        [Header("Market")] 
        [Description("If Sell true, if Buy false")] 
        [SerializeField] private bool sellOrBuy = true;
        [SerializeField] private MonoBehaviour marketComponent; // В инспекторе сюда цепляй объект с Market.cs
        private IMarket market;
        
        private void Awake()
        {
            market = marketComponent as IMarket;

            if (market == null)
            {
                Debug.LogError("Assigned component does not implement IMarket!");
            }
        }

        public string GetTextInteraction()
        {
            return sellOrBuy ? "Press F to Sell Items"  : "Press F to Buy Items";
        }

        public bool isInteractable()
        {
            return true;
        }

        public void Interact(Camera camera)
        {
            if (sellOrBuy)
            {
                Debug.Log("Button Sell Item");
                market.SellItems();
            }
            else
            {
                Debug.Log("Button Buy Item");
                market.OpenShop();
            }
            
        }
    }
}