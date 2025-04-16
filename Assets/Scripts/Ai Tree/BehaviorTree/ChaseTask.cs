using UnityEngine;
public class ChaseTask : BTNode
{
        private EnemyBase enemy;

        public ChaseTask(EnemyBase enemy)
        {
            this.enemy = enemy;
        }

        public override bool Evaluate()
        {
            float distance = Vector3.Distance(enemy.transform.position, enemy.player.position);

            float buffer = 2f;
            Vector3 direction = enemy.agent.desiredVelocity;

            if (direction.sqrMagnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                enemy.transform.rotation = Quaternion.Slerp(enemy.transform.rotation, targetRotation, Time.deltaTime * 5f);
            }

            if (distance <= enemy.detectionRange + buffer && distance > enemy.attackRange)
            {
                enemy.agent.speed = enemy.chaseSpeed;
                enemy.SetSpeed(enemy.chaseSpeed);
                enemy.agent.SetDestination(enemy.player.position);
                return true;
            }

            return false;
        }

}

