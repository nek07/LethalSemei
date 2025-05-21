// // NetworkPlayerInteraction.cs
// using Mirror;
// using UnityEngine;
// using TMPro;
// using ItemSystem;
//
// public class NetworkPlayerInteraction : NetworkBehaviour
// {
//     [Header("Interaction Settings")]
//     [SerializeField] private Camera playerCamera;
//     [SerializeField] private TextMeshProUGUI interactionText;
//     [SerializeField] private float interactionDistance = 3f;
//     private ItemRigController itemRigController;
//     private NetworkItem currentPickable;
//
//     void Start()
//     {
//         if (!isLocalPlayer)
//         {
//             enabled = false;
//             return;
//         }
//         interactionText.gameObject.SetActive(false);
//     }
//
//     void Update()
//     {
//         // 1) Находим предмет под перекрестием
//         Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
//         if (Physics.Raycast(ray, out var hit, interactionDistance))
//         {
//             var ni = hit.collider.GetComponent<NetworkItem>();
//             if (ni != null && ni.IsInteractable())
//             {
//                 currentPickable = ni;
//                 interactionText.text = ni.GetTextInteraction();
//                 interactionText.gameObject.SetActive(true);
//
//                 // 2) По нажатию E — просим сервер поднять
//                 if (Input.GetKeyDown(KeyCode.E))
//                     Debug.Log("pickup");
//                     CmdRequestPickUp(ni.netIdentity);
//                     
//                 return;
//             }
//         }
//
//         currentPickable = null;
//         interactionText.gameObject.SetActive(false);
//
//         // … сюда можно добавить бросок (G) аналогично:
//         // if (Input.GetKeyDown(KeyCode.G) && heldItem != null) CmdRequestDrop(...);
//     }
//
//     // Команда идёт именно от игрока, у которого есть authority
//     [Command]
//     private void CmdRequestPickUp(NetworkIdentity itemId)
//     {
//         var netItem = itemId.GetComponent<NetworkItem>();
//         if (netItem != null && netItem.IsInteractable())
//             netItem.ServerPickUp();
//     }
//
//     // Пример команды для броска:
//     // [Command]
//     // private void CmdRequestDrop(NetworkIdentity itemId, Vector3 pos)
//     // {
//     //     var netItem = itemId.GetComponent<NetworkItem>();
//     //     if (netItem != null) netItem.ServerDrop(pos);
//     // }
// }
