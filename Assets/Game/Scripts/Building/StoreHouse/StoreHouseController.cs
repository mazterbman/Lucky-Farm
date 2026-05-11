using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Game.Scripts.Economy;
using Game.Scripts.Items;
using Game.Scripts.Settings;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.Building.StoreHouse
{
    public class StoreHouseController : BuildingController
    {
        [Header("References")] 
        [SerializeField] private StoreHouseInterfaceController _interfaceController;
        [SerializeField] private StoreHouseUiController _uiController;
        [SerializeField] private List<StoreItem> _storeItems;
        
        [Header("Settings")] 
        [SerializeField] private int _maxItems = 100;

        [Inject] private EconomyData _economyData;
        [Inject] private SettingsLevelData _levelData;

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
                Debug.Log($"Was Add count items{item.Count}");
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
        
        public bool TryGetCountItem(TypeItem typeItem, out int count)
        {
            count = 0;
            if (_storeItems.All(storeItem => storeItem.Item.Type != typeItem)) 
                return false;

            count = _storeItems.Find(arg1 => arg1.Item.Type == typeItem).Count;
            return true;
        }

        public void ReplaceItemUi(StoreItem item, bool isRight) => _uiController.ReplaceItem(item, isRight);
        public void StartMoveTrack(int countOfMoney) => _interfaceController.MoveToSellItemsAsync(countOfMoney, DestroyTokenSource.Token).Forget();
        public void EnableInterface(bool enable) => _interfaceController.EnableInterface(enable);
        
        public BoxCollider LeftStoreCollider => _uiController?.LeftStoreCollider;
        public BoxCollider RightBoxCollider => _uiController?.RightBoxCollider;
        public Transform ParentForDrag => _uiController?.ParentForDrag;
        public Canvas Canvas => _uiController?.Canvas;
        
        public void AddOnBalance(int countOfMoney)
        {
            _economyData.BalanceLevelManager.TryAdd(countOfMoney);
            _uiController.CanSellItems(true);
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
            
            var seconds = _levelData.StoreHouseSetting.GetSecondsOnSale(_levelData.StoreHouseSetting.LevelSettingsList[levelIndex]);
            _interfaceController.SetSecondsOnSale(seconds);
        }

        private void OpenMenu()
        {
            UpdateStore();
            _uiController.Show(_storeItems);
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