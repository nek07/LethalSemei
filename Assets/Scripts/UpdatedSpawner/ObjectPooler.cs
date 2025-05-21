using System.Collections.Generic;
using UnityEngine;

namespace UpdatedSpawner
{
    public class ObjectPooler : MonoBehaviour
    {
        [System.Serializable]
        public class PoolItem
        {
            public string tag;
            public GameObject prefab;
            public int size;
        }

        public List<PoolItem> itemsToPool;
        public Dictionary<string, Queue<GameObject>> poolDictionary;

        public static ObjectPooler Instance;

        void Awake()
        {
            Instance = this;
            poolDictionary = new Dictionary<string, Queue<GameObject>>();

            foreach (var item in itemsToPool)
            {
                Queue<GameObject> objectPool = new Queue<GameObject>();
                for (int i = 0; i < item.size; i++)
                {
                    GameObject obj = Instantiate(item.prefab);
                    obj.SetActive(false);
                    objectPool.Enqueue(obj);
                }

                poolDictionary[item.tag] = objectPool;
            }
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!poolDictionary.ContainsKey(tag)) return null;

            GameObject obj = poolDictionary[tag].Dequeue();

            obj.SetActive(true);
            obj.transform.position = position;
            obj.transform.rotation = rotation;

            poolDictionary[tag].Enqueue(obj);
            return obj;
        }

    }
}