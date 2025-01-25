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
            float distance = Vector3.Distance(enemy.transform.position,enemy.player.position);
            
            if (distance <= enemy.detectionRange && distance > enemy.attackRange)
            {
                enemy.agent.speed = enemy.chaseSpeed;
                enemy.SetSpeed(enemy.chaseSpeed);
                enemy.agent.SetDestination(enemy.player.position);
                return true;
            }
            
            return false;
        }
}

