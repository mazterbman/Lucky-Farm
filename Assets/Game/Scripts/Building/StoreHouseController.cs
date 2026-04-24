using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Economy;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.Building
{
    public class StoreHouseController : BuildingController
    {
        [Header("Settings")] 
        [SerializeField] private int _maxItems = 100;

        [Header("References")]
        [SerializeField] private List<StoreItem> _storeItems;

        [Inject] private EconomyData _economyData;
        
        private BalanceLevelManager _balanceLevelManager;
        private UnityAction _onUpdateItems;

        private void Start()
        {
            _balanceLevelManager = _economyData.BalanceLevelManager;
            _onUpdateItems += UpdateStore;
        }

        public override void OnClick()
        {
            OpenMenu();
        }

        [ContextMenu("Add Egg")]
        private void AddEgg()
        {
            TryAddItem(new StoreItem().CreateEgg());
        }

        [ContextMenu("Sell Egg")]
        private void SellEgg()
        {
            TrySellItem(new StoreItem().CreateEgg());
        }

        public bool TryAddItem(StoreItem item)
        {
            if (item == null)
                return false;
            
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
            return true;
        }

        public bool TryRemoveItem(StoreItem item)
        {
            if (item == null)
                return false;
            
            if (_storeItems.All(storeItem => storeItem.Type != item.Type)) 
                return false;
            
            StoreItem storeItem = _storeItems.Find(arg1 => arg1.Type == item.Type);
            if (storeItem.Count < item.Count)
                return false;
            
            storeItem.Count -= item.Count;
            _onUpdateItems?.Invoke();
            return true;
        }

        public bool TrySellItem(StoreItem item)
        {
            if (!TryRemoveItem(item))
                return false;

            _balanceLevelManager.TryAdd(item.Coast * item.Count);
            return true;
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
        public TypeItem Type { get; set; }
        public int Count { get; set; }
        public int Coast { get; private set; }
        public string Name { get; private set; }

        public StoreItem(TypeItem type, int count)
        {
            Type = type;
            Count = count;
        }
        
        public StoreItem() { }

        public StoreItem CreateEgg()
        {
            Type = TypeItem.Egg;
            Count = 1;
            Name = "Egg";
            Coast = 15;
            
            return this;
        }

        public enum TypeItem
        {
            Egg = 0
        }
    }
}