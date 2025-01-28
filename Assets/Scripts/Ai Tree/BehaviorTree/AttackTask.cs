// AttackTask.cs
using UnityEngine;

public class AttackTask : BTNode
{
    private EnemyBase enemy;

    public AttackTask(EnemyBase enemy)
    {
        this.enemy = enemy;
    }

    public override bool Evaluate()
    {
        float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);

        if (distance <= enemy.attackRange)
        {
            // Поворачиваем NPC в сторону игрока
            Vector3 direction = enemy.player.position - enemy.transform.position;
            direction.y = 0; // Игнорируем вертикальную составляющую
            if (direction.magnitude > 0.1f) // Если направление отличается от текущего положения
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * 5f); // Плавный поворот
            }

            enemy.agent.ResetPath();
            enemy.SetAttack(true);  // Включаем анимацию атаки
            return true;
        }
        else
        {
            enemy.SetAttack(false);  // Отключаем атаку
        }

        return false;
    }
}