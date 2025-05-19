using Mirror;
using UnityEngine;
using UnityEngine.UIElements;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private UIDocument uiDoc;

    void Awake()
    {
        if (!uiDoc) uiDoc = GetComponent<UIDocument>();

        var root = uiDoc.rootVisualElement;
        var confirmBtn = root.Q<Button>("confirm-host-button");
        confirmBtn.clicked += OnPlayClicked;      // ← прямая подписка
    }

    void OnDestroy()
    {
        var confirmBtn = uiDoc.rootVisualElement.Q<Button>("confirm-host-button");
        confirmBtn.clicked -= OnPlayClicked;      // отписываемся
    }

    void OnPlayClicked()
    {
        NetworkManager.StartHost();
    }
}