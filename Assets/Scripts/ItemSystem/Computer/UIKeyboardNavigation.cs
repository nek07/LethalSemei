using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIKeyboardNavigation : MonoBehaviour
{
    public List<Button> buttons; // Список всех кнопок
    private int selectedIndex = 0;

    private void Start()
    {
        if (buttons.Count > 0)
            EventSystem.current.SetSelectedGameObject(buttons[0].gameObject);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
            MoveSelection(1);
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            MoveSelection(-1);

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            // "Нажимаем" выбранную кнопку
            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected != null)
            {
                var button = selected.GetComponent<Button>();
                if (button != null)
                    button.onClick.Invoke();
            }
        }
    }

    void MoveSelection(int direction)
    {
        selectedIndex += direction;

        if (selectedIndex < 0)
            selectedIndex = buttons.Count - 1;
        else if (selectedIndex >= buttons.Count)
            selectedIndex = 0;

        EventSystem.current.SetSelectedGameObject(buttons[selectedIndex].gameObject);
    }
}