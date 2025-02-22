using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public Camera playerCamera; // Ссылка на камеру игрока
    public float interactionDistance = 3f; // Дистанция взаимодействия
    public TextMeshProUGUI interactionText; // UI-текст

    private void Start()
    {
        interactionText.gameObject.SetActive(false); // Скрываем текст при старте
    }

    private void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null && interactable.isInteractable())
            {
                interactionText.text = interactable.GetTextInteraction();
                interactionText.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.E))
                {
                    Debug.Log("Interaction: " + hit.collider.gameObject.name);
                }
                return;
            }
        }

        // Если луч не попадает в предмет — скрываем текст
        interactionText.gameObject.SetActive(false);
    }
}

public interface IInteractable
{
    string GetTextInteraction();
    bool isInteractable();
}