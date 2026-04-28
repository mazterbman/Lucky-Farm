using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Settings;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.Building.StoreHouse
{
    public class StoreHouseController : BuildingController
    {
        [Header("Settings")] 
        [SerializeField] private int _maxItems = 100;

        [Header("References")]
        [SerializeField] private List<StoreItem> _storeItems;

        [Inject] private SettingsLevelData _levelData;
        [Inject] private BuildingData _buildingData;

        private int _currentCountItems = 0;
        private UnityAction _onUpdateItems;

        private void Start()
        {
            _onUpdateItems += UpdateStore;
        }

        public override void OnClick()
        {
            OpenMenu();
        }

        public bool TryAddItem(StoreItem item)
        {
            if (item == null)
                return false;

            if (_currentCountItems + item.Count > _maxItems)
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
        

        protected override void RemoveListeners()
        {
            _onUpdateItems -= UpdateStore;
        }

        protected override void LoadSettings(int level)
        {
            int levelIndex = Mathf.Clamp(level - 1, 0, _levelData.StoreHouseSetting.LevelSettingsList.Count - 1);
            _maxItems = _levelData.StoreHouseSetting.GetMaxItem(_levelData.StoreHouseSetting.LevelSettingsList[levelIndex]);
            _currentCountItems = 0;
        }

        private void OpenMenu()
        {
            UpdateStore();
            _buildingData.StoreHouseUiController.Show(_storeItems);
        }
        
        private void UpdateStore()
        {
            _currentCountItems = 0;
            for (int i = 0; i < _storeItems.Count; i++)
            {
                if (_storeItems[i].Count > 0)
                {
                    _currentCountItems += _storeItems.Count;
                    continue;
                }
                
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

        public StoreItem() { }
        
        public StoreItem(TypeItem type, int count)
        {
            Type = type;
            Count = count;
        }

        public StoreItem(StoreItem item)
        {
            Type = item.Type;
            Coast = item.Coast;
            Name = item.Name;
            Count = item.Count;
        }

        public StoreItem CreateEgg()
        {
            Type = TypeItem.Egg;
            Count = 1;
            Name = "Egg";
            Coast = 15;
            
            return this;
        }

        public int GetAllCoast()
        {
            return Count * Coast;
        }

        public enum TypeItem
        {
            Egg = 0
        }
    }
}