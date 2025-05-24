using UnityEngine;

namespace NTC.Global.System
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] private Transform destination;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(destination.position, .4f);
            var direction = destination.TransformDirection(Vector3.forward);
            Gizmos.DrawRay(destination.position, direction);
            
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player" && other.TryGetComponent<NetworkFirstPersonController>(out var player))
            {
                player.Teleport(destination.position,destination.rotation);
            }
        }
    }
}