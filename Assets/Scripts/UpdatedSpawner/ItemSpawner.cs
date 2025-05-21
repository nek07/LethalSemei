using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace UpdatedSpawner
{
    public class ItemSpawner : NetworkBehaviour
    {
        public List<SpawnableItem> items;
        public List<Transform> spawnPoints;
        public int budget = 5000;

        // Вес вместо % — более гибкий способ
        public Dictionary<Rarity, int> rarityWeights = new Dictionary<Rarity, int>()
        {
            { Rarity.Common, 70 },
            { Rarity.Rare, 25 },
            { Rarity.SuperRare, 5 }
        };

        public override void OnStartServer()
        {
            SpawnItems();      // (СЕТЬ) сервер создаёт предметы
        }
        [Server]
        void SpawnItems()
        {
            int remainingBudget = budget;
            List<Transform> availableSpawns = new List<Transform>(spawnPoints);

            Debug.Log("Начинаем спавн. Бюджет: $" + remainingBudget);
            Debug.Log("Доступных точек: " + availableSpawns.Count);

            while (remainingBudget > 0 && availableSpawns.Count > 0)
            {
                List<SpawnableItem> affordableItems = GetAffordableItems(remainingBudget);
                Debug.Log("Доступных предметов по цене: " + affordableItems.Count);
                if (affordableItems.Count == 0) break;

                SpawnableItem selectedItem = GetRandomItemByWeightedRarity(affordableItems);
                if (selectedItem == null)
                {
                    Debug.LogWarning("Не удалось выбрать предмет.");
                    break;
                }

                Transform spawnPoint = GetRandomSpawnPoint(availableSpawns);
                if (spawnPoint == null)
                {
                    Debug.LogWarning("Нет доступных точек.");
                    break;
                }

                Debug.Log("Спавним: " + selectedItem.prefab.name + " за $" + selectedItem.price);

                GameObject obj = ObjectPooler.Instance.SpawnFromPool(selectedItem.prefab.name, spawnPoint.position, spawnPoint.rotation);
                
                if (obj == null)
                {
                    Debug.LogError("Объект из пула не найден: " + selectedItem.prefab.name);
                }

                ApplyVisualEffect(obj, selectedItem.rarity);
                remainingBudget -= selectedItem.price;
            }

            Debug.Log("Спавн завершён. Остаток: $" + remainingBudget);
        }


        Transform GetRandomSpawnPoint(List<Transform> spawns)
        {
            if (spawns.Count == 0) return null;
            int index = Random.Range(0, spawns.Count);
            Transform point = spawns[index];
            spawns.RemoveAt(index);
            return point;
        }

        List<SpawnableItem> GetAffordableItems(int maxPrice)
        {
            return items.FindAll(item => item.price <= maxPrice);
        }

        SpawnableItem GetRandomItemByWeightedRarity(List<SpawnableItem> pool)
        {
            int totalWeight = 0;
            Dictionary<Rarity, int> weightPool = new Dictionary<Rarity, int>();

            foreach (var rarity in System.Enum.GetValues(typeof(Rarity)))
            {
                Rarity r = (Rarity)rarity;
                int weight = rarityWeights.ContainsKey(r) ? rarityWeights[r] : 0;
                weightPool[r] = weight;
                totalWeight += weight;
            }

            int roll = Random.Range(0, totalWeight);
            Rarity selected = Rarity.Common;

            foreach (var kv in weightPool)
            {
                if (roll < kv.Value)
                {
                    selected = kv.Key;
                    break;
                }

                roll -= kv.Value;
            }

            var filtered = pool.FindAll(i => i.rarity == selected);
            if (filtered.Count == 0) return null;

            return filtered[Random.Range(0, filtered.Count)];
        }

        void ApplyVisualEffect(GameObject obj, Rarity rarity)
        {
            Renderer rend = obj.GetComponent<Renderer>();
            if (rend == null) return;

            Color color = Color.white;
            switch (rarity)
            {
                case Rarity.Common:
                    color = Color.gray;
                    break;
                case Rarity.Rare:
                    color = Color.blue;
                    break;
                case Rarity.SuperRare:
                    color = Color.magenta;
                    break;
            }

            rend.material.color = color;
        }
    }
}