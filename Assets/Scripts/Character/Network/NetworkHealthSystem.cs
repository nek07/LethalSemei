
using Mirror;
using UnityEngine;

namespace NTC.Global.System
{

    public class HealthSystem : NetworkBehaviour, IDamagable
    {
        [SyncVar(hook = nameof(OnHealthChanged))]
        private float health = 100f;

        [Header("VFX & Animation")]
        [SerializeField] private GameObject hitVFX;
        [SerializeField] private Animator animator;
        private RagdollOperations ragdoll;
        private IDieable dieable;

        private void Awake()
        {
            ragdoll = GetComponent<RagdollOperations>();
            dieable = GetComponent<IDieable>();
            if (animator == null) animator = GetComponent<Animator>();
        }

        // Только сервер может менять здоровье
        [Server]
        public void TakeDamage(float damageAmount)
        {
            if (health <= 0) return;

            health -= damageAmount;
            RpcPlayHitEffects();

            if (health <= 0)
            {
                health = 0;
                RpcOnDeath();
                dieable.Die();
            }
        }

        // Синхронизируем любую внешнюю логику (например, UI) при изменении
        private void OnHealthChanged(float oldVal, float newVal)
        {
            // например, обновить полоску здоровья
            // healthBar.SetValue(newVal / maxHealth);
        }

        // Воспроизводим VFX и пускаем триггер анимации у всех клиентов
        [ClientRpc]
        private void RpcPlayHitEffects()
        {
            if (animator != null) animator.SetTrigger("damage");
            if (hitVFX != null)
            {
                var vfx = Instantiate(hitVFX, transform.position + Vector3.up * 1f, Quaternion.identity);
                Destroy(vfx, 3f);
            }
        }

        // Когда здоровье упало до нуля — включаем ragdoll у всех клиентов
        [ClientRpc]
        private void RpcOnDeath()
        {
            if (animator != null) animator.enabled = false;
            ragdoll?.EnableRagdoll();
        }

        // Не нужен HitVFX на клиенте — всё идёт через RpcPlayHitEffects()
        public void HitVFX(Vector3 hitPosition) { /* оставляем пустым */ }
    }
}