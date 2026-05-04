using System;
using Game.Scripts.Economy;
using Game.Scripts.Settings;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.Building.WaterWellController
{
    public class WaterWellController : BuildingController
    {
        [Header("References")]
        [SerializeField] private WaterWellUiController _wellUiController;

        [Header("Debug")] 
        [SerializeField] private Settings _currentSettings;
        
        [Inject] private SettingsLevelData _levelData;
        [Inject] private EconomyData _economyData;
        
        private float _currentWater;
        private UnityAction<float> _onEditWaterLevel;
        
        private void Start()
        {
            _currentWater = _currentSettings.Max;
            _onEditWaterLevel += ChangeWaterLevel;
        }

        public override void OnClick()
        {
            if (CurrentPercentWater >= 1)
                return;
            
            if (!_economyData.BalanceLevelManager.TryRemove(_currentSettings.CoastBuy))
                return;
            
            AddWater();
        }
        
        protected override void LoadSettings(int currentLevel)
        {
            int indexLevel = Mathf.Clamp(currentLevel - 1, 0, _levelData.WaterWellSetting.SettingsList.Count - 1);
            _currentSettings.Max = _levelData.WaterWellSetting.GetMaxWater(_levelData.WaterWellSetting.SettingsList[indexLevel]);
            _currentSettings.CoastBuy = _levelData.WaterWellSetting.GetCoastBuyWater(_levelData.WaterWellSetting.SettingsList[indexLevel]);
            _currentSettings.CoastUse = _levelData.WaterWellSetting.GetCoastUseWater(_levelData.WaterWellSetting.SettingsList[indexLevel]);
            _currentSettings.AddOnClk = _levelData.WaterWellSetting.GetAddWaterOnClk(_levelData.WaterWellSetting.SettingsList[indexLevel]);
        }

        protected override void RemoveListeners()
        {
            _onEditWaterLevel -= ChangeWaterLevel;
        }

        public bool TryUseWater()
        {
            if (CurrentPercentWater < _currentSettings.CoastUse)
                return false;

            _currentWater = Mathf.Clamp(_currentWater - _currentSettings.Max * _currentSettings.CoastUse, 0, _currentSettings.Max);
            _onEditWaterLevel?.Invoke(CurrentPercentWater);
            return true;
        }

        private void AddWater()
        {
            float addWater = _currentSettings.Max * _currentSettings.AddOnClk;
            _currentWater = Mathf.Clamp(_currentWater + addWater, 0, _currentSettings.Max);
            Debug.Log($"Add Water = {addWater}  Percent = {CurrentPercentWater}");
            
            _onEditWaterLevel?.Invoke(CurrentPercentWater);
        }

        private void ChangeWaterLevel(float value)
        {
            _wellUiController.UpdateBar(value);
        }

        public float CurrentPercentWater => Mathf.Clamp01(_currentWater / _currentSettings.Max);
        
        [Serializable]
        private struct Settings
        {
            public int Max;
            public int CoastBuy;
            public float CoastUse;
            public float AddOnClk;
        }
    }
}