using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class NPCSpawner : NetworkBehaviour
{
    [Header("Настройки спавна")]
    public GameObject npcPrefab; // Префаб NPC (должен быть зарегистрирован в NetworkManager)
    public float spawnInterval = 10f; // Интервал спавна
    public int maxNPCCount = 10; // Максимум активных NPC

    [Header("Точки спавна")]
    public List<Transform> spawnPoints = new List<Transform>();

    private float timer = 0f;

    private List<GameObject> spawnedNPCs = new List<GameObject>();

    void Update()
    {
        if (!isServer) return; // Спавн должен работать только на сервере

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnNPCIfNeeded();
        }
    }
    private List<Transform> GetGlobalPatrolPoints()
    {
        var points = new List<Transform>();
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("PatrolPoint"))
        {
            points.Add(obj.transform);
        }
        return points;
    }

    void SpawnNPCIfNeeded()
    {
        // Удаляем мертвых NPC из списка
        spawnedNPCs.RemoveAll(npc => npc == null);

        if (spawnedNPCs.Count >= maxNPCCount)
            return;

        if (spawnPoints.Count == 0)
        {
            Debug.LogWarning("Нет точек спавна для NPC.");
            return;
        }

        // Выбираем случайную точку
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        GameObject npc = Instantiate(npcPrefab, spawnPoint.position, spawnPoint.rotation);
        // EnemyBase zombie = npc.GetComponent<EnemyBase>();
        EnemyBase enemy = npc.GetComponent<EnemyBase>();

// находим патрульные точки и передаем их
        List<Transform> patrolPoints = new List<Transform>();
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("PatrolPoint"))
        {
            patrolPoints.Add(go.transform);
        }

        
        NetworkServer.Spawn(npc);
        enemy.setPatrolPoints(patrolPoints);

// теперь запускаем патрулирование
        enemy.InitPatrol(); // метод, который мы добавим ниже

        // if (zombie != null)
        // {
        //     zombie.setPatrolPoints(GetGlobalPatrolPoints());
        // }
        spawnedNPCs.Add(npc);
    }
}