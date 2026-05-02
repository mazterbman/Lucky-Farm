using System;
using Game.Scripts.Items;

namespace Game.Scripts.Building.StoreHouse
{
    [Serializable]
    public class StoreItem
    {
        public StoreItem(StoreItem storeItem)
        {
            Item = new (storeItem.Item);
            Count = storeItem.Count;
        }

        public StoreItem(Item item, int count)
        {
            Item = new (item);
            Count = count;
        }

        public Item Item { get; set; }
        public int Count { get; set; }

        public int GetAllCoast()
        {
            return Count * Item.Coast;
        }
    }
}