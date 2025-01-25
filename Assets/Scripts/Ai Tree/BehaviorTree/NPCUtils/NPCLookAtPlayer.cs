using UnityEngine;

public class NPCLookAtPlayer : MonoBehaviour
{
    public Transform player; // Ссылка на трансформ игрока
    public float detectionRange = 10f; // Максимальная дистанция видимости
    public LayerMask obstacleLayer; // Слой препятствий (например, стены)

    void Update()
    {
        if (player != null)
        {
            // Направление от NPC к игроку
            Vector3 direction = player.position - transform.position;

            // Проверка, есть ли препятствия между NPC и игроком
            RaycastHit hit;
            if (Physics.Raycast(transform.position, direction, out hit, detectionRange, obstacleLayer))
            {
                // Если столкновение с препятствием (стеной или объектом)
                if (hit.collider.gameObject != player.gameObject)
                {
                    return; // Не смотреть на игрока, если между ним и NPC есть препятствие
                }
            }

            // Поворачиваем NPC в сторону игрока, если нет препятствий
            direction.y = 0; // Игнорируем вертикаль
            Quaternion rotation = Quaternion.LookRotation(direction); // Рассчитываем нужное вращение
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f); // Плавное вращение
        }
    }
}