using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Building.StoreHouse;
using Game.Scripts.Items;
using Game.Scripts.Settings;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Building.SweetShop
{
    public class SweetShopController : BuildingController
    {
        [Header("References")]
        [SerializeField] private SweetShopUiController _shopUiController;
        
        [Header("Settings")] 
        [SerializeField] private SweepShopItem _itemIn;
        [SerializeField] private SweepShopItem _itemOut;
        
        [Inject] private SettingsLevelData _levelData;
        [Inject] private BuildingData _buildingData;

        private CancellationTokenSource _tokenSource;
        private int _timeWaite = 0;
        private State _currentState = State.IsEmpty;

        public override void OnClick()
        {
            switch (_currentState)
            {
                case State.IsEmpty:
                    StateEmpty();
                    break;
                
                case State.IsFinish:
                    StateFinish();
                    break;
                
                case State.InProgress:
                default:
                    break;
            }
        }

        protected override void RemoveListeners()
        {
        }

        protected override void LoadSettings(int level)
        {
            int levelIndex = Mathf.Clamp(level - 1, 0, _levelData.StoreHouseSetting.LevelSettingsList.Count - 1);
            _itemOut.Count = _levelData.SweetShopSetting.GetMaxItem(_levelData.SweetShopSetting.LevelSettingsList[levelIndex]);
            _timeWaite = _levelData.SweetShopSetting.GetTimeWaite(_levelData.SweetShopSetting.LevelSettingsList[levelIndex]);
        }

        private void StateEmpty()
        {
            if (!_buildingData.StoreHouseController.TryRemoveItem(new StoreItem(_itemIn.Item, _itemIn.Count)))
                return;
            
            _currentState = State.InProgress;
            WaiteItemAsync(DestroyTokenSource.Token).Forget();
        }

        private void StateFinish()
        {
            if (!_buildingData.StoreHouseController.TryAddItem(new StoreItem(_itemOut.Item, _itemOut.Count)))
                return;

            _currentState = State.IsEmpty;
            _shopUiController.UpdateBar(0);
        }

        private async UniTask WaiteItemAsync(CancellationToken token)
        {
            float timePast = 0;
            while (timePast < _timeWaite && !token.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                timePast += Time.deltaTime;
                _shopUiController.UpdateBar(timePast / _timeWaite);
            }

            if (token.IsCancellationRequested)
                return;
            
            _currentState = State.IsFinish;
            Debug.Log("<color=green>FINISH</color>");
        }
        
        public enum State
        {
            IsEmpty = 0,
            InProgress,
            IsFinish
        }
        
        [Serializable]
        private class SweepShopItem
        {
            public Item Item;
            public int Count;
        }
    }
}
