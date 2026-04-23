using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Building
{
    public class StoreHouseController : BuildingController
    {
        [Header("Settings")] 
        [SerializeField] private int _maxItems = 100;

        [Header("References")]
        [SerializeField] private List<StoreItem> _storeItems;

        private UnityAction _onUpdateItems;

        private void Start()
        {
            _onUpdateItems += UpdateStore;
        }

        public override void OnClick()
        {
            OpenMenu();
        }

        [ContextMenu("Add Egg")]
        private void AddEgg()
        {
            AddItem(new StoreItem(StoreItem.TypeItem.Egg, 1, "Egg"));
        }

        [ContextMenu("Remove Egg")]
        private void RemoveEgg()
        {
            RemoveItem(new StoreItem(StoreItem.TypeItem.Egg, 1, "Egg"));
        }

        public void AddItem(StoreItem item)
        {
            if (item == null)
                return;
            
            if (_storeItems.All(storeItem => storeItem.Type != item.Type))
            {
                _storeItems.Add(item);
            }
            else
            {
                StoreItem storeItem = _storeItems.Find(arg1 => arg1.Type == item.Type);
                storeItem.Count += item.Count;
            }
            
            _onUpdateItems?.Invoke();
        }

        public void RemoveItem(StoreItem item)
        {
            if (item == null)
                return;
            
            if (_storeItems.All(storeItem => storeItem.Type != item.Type)) 
                return;
            
            StoreItem storeItem = _storeItems.Find(arg1 => arg1.Type == item.Type);
            if (storeItem.Count < item.Count)
                return;
            
            storeItem.Count -= item.Count;
            _onUpdateItems?.Invoke();
        }
        

        protected override void RemoveListeners()
        {
            _onUpdateItems -= UpdateStore;
        }

        private void OpenMenu()
        {
            UpdateStore();
        }
        
        private void UpdateStore()
        {
            for (int i = 0; i < _storeItems.Count; i++)
            {
                if (_storeItems[i].Count > 0) continue;
                
                _storeItems.RemoveAt(i);
                i--;
            }
        }
    }

    [Serializable]
    public class StoreItem
    {
        public TypeItem Type;
        public int Count;
        public string Name;

        public StoreItem(TypeItem type, int count, string name)
        {
            Type = type;
            Count = count;
            Name = name;
        }

        public enum TypeItem
        {
            Egg = 0
        }
    }
}