using System;
using Game.Scripts.Items;
using Game.Scripts.Settings;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Building.SweetShop
{
    public class SweetShopController : BuildingController
    {
        [Header("References")]
        [SerializeField]
        private SweetShopUiController _shopUiController;
        
        [Header("Settings")] 
        [SerializeField] private SweetShopItem _itemIn;
        [SerializeField] private SweetShopItem _itemOut;
        
        [Inject] private SettingsLevelData _levelData;
        
        private int _timeWaite = 0;
        private State _currentState = State.IsEmpty;

        public override void OnClick()
        {
            //ToDo
            //Take 2 Eggs -> waite time -> Make 1 omelet
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
        
        public enum State
        {
            IsEmpty = 0,
            InProgress,
            IsFinish
        }
        
        [Serializable]
        private struct SweetShopItem
        {
            public TypeItem Type;
            public int Count;
        }
    }
}
