// PatrolTask.cs
using UnityEngine;

public class PatrolTask : BTNode
{
    private EnemyBase enemy;
    private int currentPatrolIndex = 0; // Индекс текущей точки патруля
    private Vector3 patrolPoint;

    public PatrolTask(EnemyBase enemy)
    {
        this.enemy = enemy;
        SetNewPatrolPoint();
    }

    private void SetNewPatrolPoint()
    {
        // Переход к следующей точке патруля в списке
        if (enemy.patrolPoints.Count > 0)
        {
            patrolPoint = enemy.patrolPoints[currentPatrolIndex].position;
            currentPatrolIndex = (currentPatrolIndex + 1) % enemy.patrolPoints.Count; // Переход к следующей точке в цикле
        }
    }

    public override bool Evaluate()
    {
        enemy.agent.speed = enemy.patrolSpeed;
        enemy.SetSpeed(enemy.patrolSpeed);  // Включаем анимацию ходьбы
        Vector3 direction = enemy.agent.desiredVelocity;

        if (direction.sqrMagnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * 5f);
        }
        // Поворачиваем NPC в сторону патрульной точки
        // Vector3 direction = patrolPoint - enemy.transform.position;
        //direction.y = 0; // Игнорируем вертикальную составляющую
        //if (direction.magnitude > 0.1f) // Если направление отличается от текущего положения
      //  {
      //      Quaternion targetRotation = Quaternion.LookRotation(direction);
     //       enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * 5f); // Плавный поворот
     //   }
        

        // Если NPC достиг патрульной точки, устанавливаем новую точку
        if (Vector3.Distance(enemy.transform.position, patrolPoint) < 1f)
            SetNewPatrolPoint();

        enemy.agent.SetDestination(patrolPoint);

        // Если игрок в радиусе обнаружения, переключаемся на преследование
        if (Vector3.Distance(enemy.transform.position, enemy.player.position) <= enemy.detectionRange)
            return false;  // Переключиться на преследование

        return true;
    }
}