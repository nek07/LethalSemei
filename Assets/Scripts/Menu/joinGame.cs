using UnityEngine;
using UnityEngine.UI;
using TMPro;           // или using UnityEngine.UI если вы юзаете обычный InputField
using Mirror;
using Mirror.Transports;  // для доступа к портам в TelepathyTransport и пр.

public class joinGame : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField addressInput;  // поле ввода вида "IP:PORT"
    public Button confirmButton;         // кнопка Confirm

    [Header("Mirror Components")]
    public NetworkManager networkManager;    // ваш NetworkManager
    public Transport transportLayer;         // например TelepathyTransport

    void Awake()
    {
        confirmButton.onClick.AddListener(OnConfirm);
    }

    void OnConfirm()
    {
        string raw = addressInput.text.Trim();
        if (string.IsNullOrEmpty(raw))
        {
            Debug.LogError("Address field is empty!");
            return;
        }

        // Попробуем разбить по двоеточию
        var parts = raw.Split(':');
        if (parts.Length != 2)
        {
            Debug.LogError("Неверный формат. Введите IP и Port через ':'");
            return;
        }

        string ip = parts[0];
        string portStr = parts[1];

        // Настраиваем IP
        networkManager.networkAddress = ip;

        // Парсим порт
        if (!ushort.TryParse(portStr, out ushort port))
        {
            Debug.LogError($"Не удалось распознать порт: {portStr}");
            return;
        }

        // Передаём порт в транспорт (здесь пример для TelepathyTransport)
        if (transportLayer is TelepathyTransport tp)
        {
            tp.port = port;
        }
        else
        {
            // Если другой транспорт — посмотрите его API
            Debug.LogWarning("Transport не Telepathy, не настроил порт автоматически.");
        }

        // Запускаем клиент
        networkManager.StartClient();
        Debug.Log($"Connecting to {ip}:{port} …");
    }
}