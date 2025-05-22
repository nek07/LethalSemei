using UnityEngine;

public class IdleTask : BTNode
{
    private EnemyBase enemy;
    private Transform player;
    private float viewAngle;

    public IdleTask(EnemyBase enemy, Transform player, float viewAngle = 70f)
    {
        this.enemy = enemy;
        this.player = player;
        this.viewAngle = viewAngle;
    }

    public override bool Evaluate()
    {
        // Повторная проверка угла (как в IsPlayerLookingTask)
        Vector3 dirToEnemy = enemy.transform.position - player.position;
        float angle = Vector3.Angle(player.forward, dirToEnemy);

        if (angle > viewAngle)
        {
            // Игрок больше не смотрит — не застываем
            return false;
        }

        enemy.StopMovement();
        return true;
    }
}