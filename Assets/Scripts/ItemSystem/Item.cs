using UnityEngine;

namespace ItemSystem
{
    public class Item : MonoBehaviour, IPickable
    {
        public ItemSO itemSO;
        public GameObject prefab;

        public Item(ItemSO itemSO, GameObject prefab)
        {
            this.itemSO = itemSO;
            this.prefab = prefab;
        }


        public Item GetItem()
        {
            return this;
        }

        public void OnPickItem()
        {
            Debug.Log("SimpleItem.OnPickItem");
            //Destroy(gameObject);
        }
    }
}