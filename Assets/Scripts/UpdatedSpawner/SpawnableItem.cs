using UnityEngine;

namespace UpdatedSpawner
{
    public enum Rarity
    {
        Common,
        Rare,
        SuperRare
    }

    [System.Serializable]
    public class SpawnableItem
    {
        public GameObject prefab;
        public int price;
        public Rarity rarity;
    }
}