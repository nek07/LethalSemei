using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject joinGamePanel;

    // Вызывается по клику на кнопку Join Game в главном меню
    public void ShowJoinGame()
    {
        mainMenuPanel.SetActive(false);
        joinGamePanel.SetActive(true);
    }

    // Вызывается по клику на кнопку Back внутри окна Join Game
    public void HideJoinGame()
    {
        joinGamePanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}