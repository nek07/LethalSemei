using UnityEngine;
using ItemSystem;

namespace ItemSystem
{
    public class GeigerCounterItem : Item
    {
        public float detectionRadius = 10f;
        public LayerMask itemLayer; // Назначь слой, в котором находятся радиоактивные предметы
        public AudioSource audioSource; // Щелкающий звук
        [SerializeField] private AudioSource buttonAudio;

        public GeigerCounterItem(ItemSO itemSO, Rigidbody rb, CharacterAnimController characterAnimController, bool isIntreactable, float detectionRadius, LayerMask itemLayer, AudioSource audioSource) : base(itemSO, rb, characterAnimController, isIntreactable)
        {
            this.detectionRadius = detectionRadius;
            this.itemLayer = itemLayer;
            this.audioSource = audioSource;
        }

        private void Update()
        {
            base.Update(); // На всякий случай вызываем базовое поведение
            if (isIntreactable)
            {
                audioSource.volume = 0;
                return;
            }
            Item closestItem = FindClosestRadioactiveItem();
            if (closestItem != null)
            {
                
                float distance = Vector3.Distance(transform.position, closestItem.transform.position);
                float targetRadiation = closestItem.radiation;

                float intensity = Mathf.Clamp01(targetRadiation / 100f); // нормализуем
                float proximity = Mathf.Clamp01(1f - distance / detectionRadius);

                float volume = intensity * proximity;
                audioSource.volume = volume;
                Debug.Log("Volume: " + volume);
            }
            else
            {
                audioSource.volume = 0f;
            }
        }
    
        private Item FindClosestRadioactiveItem()
        {
            Debug.Log("Start poisk");
            Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, itemLayer);
            Item closest = null;
            float minDistance = float.MaxValue;

            foreach (var hit in hits)
            {
                if (hit.gameObject == this.gameObject) continue; // Не обрабатываем сам себя

                Item item = hit.GetComponent<Item>();
                if (item != null && item.radiation > 0)
                {
                    float dist = Vector3.Distance(transform.position, item.transform.position);
                    Debug.Log("Distance: " + dist + "/nRadiation " + item.radiation);
                    if (dist < minDistance)
                    {
                        closest = item;
                        minDistance = dist;
                    }
                }
            }

            return closest;
        }
        public override void OnDropItem()
        {
            base.OnDropItem();
            isIntreactable = true;
            if (buttonAudio != null)
            {
                buttonAudio.Play();
            }
        }
    }
}