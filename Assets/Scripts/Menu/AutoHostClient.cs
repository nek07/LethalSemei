using UnityEngine;
using Mirror;

namespace MirrorBasics
{
    public class AutoHostClient : MonoBehaviour
    {
        [SerializeField] private NetworkManager networkManager;

        void Start()
        {
            if (networkManager == null)
            {
                Debug.LogError("❌ NetworkManager не назначен в инспекторе!");
                return;
            }

            if (!Application.isBatchMode) // Клиентский режим
            {
                Debug.Log("=== Client Build ===");
                Debug.Log($"Connecting to server at {networkManager.networkAddress}:{((TelepathyTransport)networkManager.transport).port}");
                networkManager.StartClient();
            }
            else // Серверный режим
            {
                Debug.Log("=== Server Build ===");

                if (!networkManager.isNetworkActive)
                {
                    networkManager.StartServer();
                    Debug.Log("✅ Сервер успешно запущен!");

                    // Ждём 1 секунду перед сменой сцены (даём время серверу инициализироваться)
                    Invoke(nameof(SwitchToOnlineScene), 1f);
                }
            }
        }

        private void SwitchToOnlineScene()
        {
            if (!string.IsNullOrEmpty(networkManager.onlineScene))
            {
                Debug.Log($"🔄 Переход на онлайн-сцену: {networkManager.onlineScene}");
                networkManager.ServerChangeScene(networkManager.onlineScene);
            }
            else
            {
                Debug.LogWarning("❌ onlineScene не задана в NetworkManager!");
            }
        }

        public void JoinLocal()
        {
            if (networkManager == null)
            {
                Debug.LogError("❌ NetworkManager не назначен в инспекторе!");
                return;
            }

            networkManager.networkAddress = "localhost";
            networkManager.StartClient();
        }
    }
}