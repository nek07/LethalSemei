using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ItemSystem.Market
{
    public class ShopItemUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private Image icon;
        [SerializeField] private bool destroyOnBuy = true;
        [SerializeField] public Button selectable;
        private IMarket market;
        private UIKeyboardNavigation navigation;
        //private GameObject itemPrefab;
        int price = 0;
        string itemName = "";

        public void Initialize(string name, Sprite icon, int price, IMarket market, UIKeyboardNavigation keyboardNavigation)
        {
            //this.itemPrefab = itemPrefab;
            title.text = name + "\n" + price.ToString();
            this.icon.sprite = icon;
            this.price = price;
            this.itemName = name;
            this.market = market;
            navigation = keyboardNavigation;
        }

        /*public GameObject GetItemPrefab()
        {
            return itemPrefab;
        }
        */

        public void TryToBuy()
        {
            if (TeamManager.Instance.GetMoney() < price)
                return;
            
            market.Buy(itemName);

            if (destroyOnBuy)
            {
                if (navigation != null && selectable != null)
                {
                    navigation.buttons.Remove(selectable);
                }

                Destroy(gameObject);
            }
        }
       

    }
}