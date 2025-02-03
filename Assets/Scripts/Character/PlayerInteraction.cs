using System;
using ItemSystem;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour{
    [SerializeField] private float interactionDistance = 3f;
    [SerializeField] private LayerMask interactionLayerMask;
    private Item item;

    private void Update()
    {
        CheckForInteraction();
    }

    private void CheckForInteraction()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionLayerMask))
        {
            
        }
    }
}