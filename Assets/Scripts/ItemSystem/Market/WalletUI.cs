using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace ItemSystem.Market
{
    public class WalletUI:MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI interactionText; // UI-текст
        private void Update()
        {
            interactionText.text = "Tenge - " + TeamManager.Instance.GetMoney();
        }
    }
}