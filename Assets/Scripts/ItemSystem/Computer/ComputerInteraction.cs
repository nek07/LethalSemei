using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

public class ComputerInteraction : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform computerViewPoint;  // Точка, где будет камера
    [SerializeField] private float transitionDuration = 1.0f;  // Время перемещения
    [SerializeField] private MonoBehaviour[] functions;
    [SerializeField] private string interactionText = "Press F to interact with the computer";
    [SerializeField] private UIKeyboardNavigation navigation;

    private Camera playerCamera;
    private CinemachineBrain cinemachineBrain;
    private FirstPersonController playerControllerScript; // Скрипт движения
    private KeyCode exitKey = KeyCode.Escape;

    private bool inComputer = false;
    private bool isTransitioning = false;
    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private void Update()
    {
        if (Input.GetKeyDown(exitKey) && inComputer && !isTransitioning)
        {
            ExitComputer();
        }
    }

    public string GetTextInteraction()
    {
        return interactionText;
    }

    public bool isInteractable()
    {
        return !inComputer && !isTransitioning;
    }

    public void Interact(Camera playerCam)
    {
        playerCamera = playerCam;
        playerControllerScript = playerCam.GetComponentInParent<FirstPersonController>();
        cinemachineBrain = playerCamera.GetComponent<CinemachineBrain>();
        StartCoroutine(MoveToComputerView());
    }

    IEnumerator MoveToComputerView()
    {
        isTransitioning = true;
        inComputer = true;
        
        EventSystem.current.SetSelectedGameObject(null);
        navigation.enabled = true;
        navigation.ActivateButtons();
        if (cinemachineBrain != null)
            cinemachineBrain.enabled = false;

        if (playerControllerScript != null)
            playerControllerScript.enabled = false;
        
        foreach (MonoBehaviour function in functions)
            function.enabled = true;
        

        originalPosition = playerCamera.transform.position;
        originalRotation = playerCamera.transform.rotation;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            playerCamera.transform.position = Vector3.Lerp(originalPosition, computerViewPoint.position, t);
            playerCamera.transform.rotation = Quaternion.Slerp(originalRotation, computerViewPoint.rotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.transform.position = computerViewPoint.position;
        playerCamera.transform.rotation = computerViewPoint.rotation;

        isTransitioning = false;
    }

    void ExitComputer()
    {
        StartCoroutine(MoveBackToPlayerView());
    }

    IEnumerator MoveBackToPlayerView()
    {
        isTransitioning = true;

        Vector3 startPos = playerCamera.transform.position;
        Quaternion startRot = playerCamera.transform.rotation;

        float elapsed = 0f;
        while (elapsed < transitionDuration)
        {
            float t = elapsed / transitionDuration;
            playerCamera.transform.position = Vector3.Lerp(startPos, originalPosition, t);
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, originalRotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.transform.position = originalPosition;
        playerCamera.transform.rotation = originalRotation;

        if (playerControllerScript != null)
            playerControllerScript.enabled = true;

        if (cinemachineBrain != null)
            cinemachineBrain.enabled = true;

        foreach (MonoBehaviour function in functions)
            function.enabled = false;
        

        inComputer = false;
        isTransitioning = false;
        
        EventSystem.current.SetSelectedGameObject(null);
        navigation.DiactivateButtons();
        navigation.enabled = false;
    }
}
