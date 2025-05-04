using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ItemSystem.Market
{
    public class ShopUI : MonoBehaviour
    {
        [SerializeField] ShopItemUI itemUIPrefab;
        [SerializeField] Transform containerUI;
        [SerializeField] private GameObject shop;
        [SerializeField] private MonoBehaviour marketComponent;
        [SerializeField] private UIKeyboardNavigation keyboardNavigation;
        private IMarket market;
        private void Awake()
        {
            market = marketComponent as IMarket;

            if (market == null)
            {
                Debug.LogError("Assigned component does not implement IMarket!");
            }
        }
        public void OnExit()
        {
            Cursor.visible = false;
            shop.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void CreateItemUI(List<Item> items)
        {
            foreach (var item in items)
            {
                ShopItemUI itemUI = Instantiate(itemUIPrefab, shop.transform);
                itemUI.transform.SetParent(containerUI);
                itemUI.Initialize(item.itemSO.itemName, item.itemSO.itemSprite, item.itemSO.maxPrice, market, keyboardNavigation);
                keyboardNavigation.buttons.Add(itemUI.GetComponent<Button>());
            }
        }

        public void ShowShop()
        {
            shop.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}