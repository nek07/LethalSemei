// // NetworkItem.cs
// using Mirror;
// using UnityEngine;
//
// namespace ItemSystem
// {
//     public interface INetworkPickable : IInteractable
//     {
//         // Клиентский вызов → теперь только запрос, без [Command]
//         void OnPickItem();
//         void OnDropItem();
//         
//     }
//
//     public class NetworkItem : NetworkBehaviour, INetworkPickable
//     {
//         
//         [Header("Item Data")]
//         public ItemSO itemSO;
//
//         [Header("Dependencies")]
//         [SerializeField] public Rigidbody rb;
//         [SerializeField] public NetworkPersonAnimationController characterAnimController;
//
//         [SyncVar] private bool _isInteractable = true;
//
//         private void Awake()
//         {
//             if (rb == null) rb = GetComponent<Rigidbody>();
//         }
//         public NetworkItem GetItem()
//         {
//             return this;
//         }
//
//         public override void OnStartClient()
//         {
//             base.OnStartClient();
//             gameObject.SetActive(_isInteractable);
//         }
//
//         // вызывается на клиенте — просто покидает запрос к игроку
//         public void OnPickItem()
//         {
//             if (!_isInteractable) return;
//             // Ничего не делаем, команду отправляем из PlayerInteraction
//         }
//
//         // выполняется только на сервере
//         [Server]
//         public void ServerPickUp()
//         {
//             if (!_isInteractable) return;
//             _isInteractable = false;
//             rb.isKinematic   = true;
//             RpcOnPickedUp();
//         }
//         public void SetCharacterAnimController(NetworkPersonAnimationController characterAnimController)
//         {
//             this.characterAnimController = characterAnimController;
//         }
//         [ClientRpc]
//         protected virtual void RpcOnPickedUp()
//         {
//             // gameObject.SetActive(true);
//             //false
//         }
//
//         public void OnDropItem( )
//         {
//             if (!isLocalPlayer) return;
//             // команду отправит игрок
//         }
//
//         [Server]
//         public void ServerDrop(Vector3 pos)
//         {
//             _isInteractable = true;
//             rb.isKinematic  = false;
//             RpcOnDropped(pos);
//         }
//
//         [ClientRpc]
//         protected virtual void RpcOnDropped(Vector3 pos)
//         {
//             transform.position = pos;
//             rb.velocity        = Vector3.zero;
//             gameObject.SetActive(true);
//         }
//
//         public bool IsInteractable() => _isInteractable;
//
//         public string GetTextInteraction() =>
//             $"Press E to pick up {itemSO.name}";
//     }
// }
