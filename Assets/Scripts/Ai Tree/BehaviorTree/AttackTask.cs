// AttackTask.cs
using UnityEngine;

public class AttackTask : BTNode
{
    private EnemyBase enemy;
    private float lastAttackTime = 0f;

    public AttackTask(EnemyBase enemy)
    {
        this.enemy = enemy;
    }

    public override bool Evaluate()
    {
        if (enemy.player == null)
        {
            enemy.SetAttack(false);
            return false;
        }

        float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);

        if (distance <= enemy.attackRange)
        {
            // Поворачиваем NPC в сторону игрока
            Vector3 direction = enemy.player.position - enemy.transform.position;
            direction.y = 0;

            if (direction.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            enemy.agent.ResetPath();
            enemy.SetAttack(true);

            if (Time.time - lastAttackTime >= enemy.attackCooldown)
            {
                lastAttackTime = Time.time;
                TryAttack();
            }

            return true;
        }
        else
        {
            // Игрок ушёл – сбрасываем атаку и позволяем BT переключиться на погоню
            enemy.SetAttack(false);
            return false;
        }
    }


    
    private void TryAttack()
    {
        
        IDamagable target = enemy.player.GetComponent<IDamagable>();
        if (target != null)
        {
            Debug.Log("Попытка атаки");
            target.TakeDamage(enemy.attackDamage);
            target.HitVFX(enemy.player.transform.position);
        }
    }
}