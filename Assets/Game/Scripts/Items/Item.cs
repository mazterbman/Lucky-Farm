using UnityEngine;

namespace Game.Scripts.Items
{
    [CreateAssetMenu(fileName = "ItemType", menuName = "Scriptable Objects/Items/ItemType")]
    public class Item : ScriptableObject
    {
        public TypeItem Type;
        public int Coast;

        public Item(Item item)
        {
            Type = item.Type;
            Coast = item.Coast;
        }
    }

    public enum TypeItem
    {
        Egg = 0,
        Omelet,
        Chicken,
    }
}
