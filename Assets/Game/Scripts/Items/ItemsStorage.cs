using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Scripts.Items
{
    [CreateAssetMenu(fileName = "ItemStorage", menuName = "Scriptable Objects/Items/ItemStorage")]
    public class ItemsStorage : ScriptableObject
    {
        public List<Item> _items;

        public bool TryGetItem(TypeItem type, out Item item)
        {
            item = null;
            if (_items.Any(itm => itm.Type == type))
            {
                item = _items.Find(itm => itm.Type == type);
                return true;
            }

            return false;
        }
    }
}