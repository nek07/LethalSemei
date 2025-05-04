using UnityEngine;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public Camera playerCamera; // Ссылка на камеру игрока
    public float interactionDistance = 3f; // Дистанция взаимодействия
    public LayerMask interactionMask;
    public TextMeshProUGUI interactionText; // UI-текст

    private void Start()
    {
        interactionText.gameObject.SetActive(false); // Скрываем текст при старте
    }

    private void Update()
    {
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        Vector3 rayOrigin = playerCamera.transform.position;
        Vector3 rayDirection = playerCamera.transform.forward;

        // Рисует линию, но только в режиме Scene
        Debug.DrawRay(rayOrigin, rayDirection * 5f, Color.red);
        if (Physics.Raycast(ray, out hit, interactionDistance, interactionMask))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null && interactable.isInteractable())
            {
                
                interactionText.text = interactable.GetTextInteraction();
                interactionText.gameObject.SetActive(true);

                if (Input.GetKeyDown(KeyCode.F))
                {
                    interactable.Interact(playerCamera);
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
    void Interact(Camera camera);
}