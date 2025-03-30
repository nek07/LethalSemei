using UnityEngine;

namespace Setting
{
    public class Portal : MonoBehaviour
    {
        [SerializeField] Transform destination;

        void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(destination.position, .4f);
            var direction = destination.TransformDirection(Vector3.forward);
            Gizmos.DrawRay(destination.position, direction);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && other.TryGetComponent<FirstPersonController>(out var player))
            {
                player.Teleport(destination.position, destination.rotation);
            }
        }
    }
}