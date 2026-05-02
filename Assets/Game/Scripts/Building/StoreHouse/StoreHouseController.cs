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
            
            if (_storeItems.All(storeItem => storeItem.Item.Type != item.Item.Type))
            {
                _storeItems.Add(item);
            }
            else
            {
                StoreItem storeItem = _storeItems.Find(arg1 => arg1.Item.Type == item.Item.Type);
                storeItem.Count += item.Count;
            }
            
            _onUpdateItems?.Invoke();
            return true;
        }
        
        public bool TryRemoveItem(StoreItem item)
        {
            if (item == null)
                return false;
            
            if (_storeItems.All(storeItem => storeItem.Item.Type != item.Item.Type)) 
                return false;
            
            StoreItem storeItem = _storeItems.Find(arg1 => arg1.Item.Type == item.Item.Type);
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
}