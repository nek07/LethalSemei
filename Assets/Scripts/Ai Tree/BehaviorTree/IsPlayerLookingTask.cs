using UnityEngine;
using System.Collections;

namespace Ai_Tree
{
    public class IsPlayerLookingTask : BTNode
    {
        private EnemyBase enemy;
        private Transform player;
        private float viewAngle;
        private float eyeHeight;
        private LayerMask obstacleMask;

        public bool IsVisible { get; private set; }

        public IsPlayerLookingTask(EnemyBase enemy, Transform player, float viewAngle = 70f, float eyeHeight = 1.6f,
            LayerMask obstacleMask = default)
        {
            this.enemy = enemy;
            this.player = player;
            this.viewAngle = viewAngle;
            this.eyeHeight = eyeHeight;
            this.obstacleMask = obstacleMask;
        }

        public override bool Evaluate()
        {
            // Проверка, находится ли противник в поле зрения камеры
            Camera cam = Camera.main;
            Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);

            Bounds enemyBounds = enemy.GetComponentInChildren<Renderer>().bounds;

            // Если противник не в поле зрения камеры, он точно не видим
            if (!GeometryUtility.TestPlanesAABB(planes, enemyBounds))
            {
                Debug.Log("Не видно");
                IsVisible = false;
                return false;
            }

            // Проверяем луч между позицией игрока и противником
            Vector3 playerEyePosition = player.position + Vector3.up * eyeHeight;
            Vector3 directionToEnemy = (enemy.transform.position - playerEyePosition).normalized;
            float distanceToEnemy = Vector3.Distance(playerEyePosition, enemy.transform.position);

            // Проверка, смотрит ли игрок примерно в направлении противника
            float angleToEnemy = Vector3.Angle(cam.transform.forward, directionToEnemy);
            if (angleToEnemy > viewAngle / 2)
            {
                Debug.Log("Не видно не в напрявление игрока");
                IsVisible = false;
                return false;
            }

            // Проверяем, нет ли препятствий между игроком и противником
            if (Physics.Raycast(playerEyePosition, directionToEnemy, out RaycastHit hit, distanceToEnemy, obstacleMask))
            {
                Debug.DrawRay(playerEyePosition, directionToEnemy * distanceToEnemy, Color.yellow);
                // Есть препятствие между игроком и противником
                Debug.Log("припяствие мешает: " + hit.collider.name);
                IsVisible = false;
                return false;
            }

            // Если дошли до этой точки, значит противник видим игроку
            Debug.Log("видит");
            IsVisible = true;
            return true;
        }
    }
}