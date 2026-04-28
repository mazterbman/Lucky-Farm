using System;
using Game.Scripts.Economy;
using Game.Scripts.Settings;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.Building
{
    public class WaterWellController : BuildingController
    {
        [Header("References")]
        [SerializeField] private ResourceBar _resourceBar;

        [Header("Debug")] 
        [SerializeField] private Settings _currentSettings;
        
        [Inject] private SettingsLevelData _levelData;
        [Inject] private EconomyData _economyData;
        
        private float _currentWater;
        private UnityAction<float> _onEditWaterLevel;
        
        private void Start()
        {
            _currentWater = _currentSettings.Max;
            _resourceBar.ResetBar();
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
            int indexLevel = Mathf.Clamp(currentLevel - 1, 0, _levelData.WaterWellSettings.SettingsList.Count - 1);
            _currentSettings.Max = _levelData.WaterWellSettings.GetMaxWater(_levelData.WaterWellSettings.SettingsList[indexLevel]);
            _currentSettings.CoastBuy = _levelData.WaterWellSettings.GetCoastBuyWater(_levelData.WaterWellSettings.SettingsList[indexLevel]);
            _currentSettings.CoastUse = _levelData.WaterWellSettings.GetCoastUseWater(_levelData.WaterWellSettings.SettingsList[indexLevel]);
            _currentSettings.AddOnClk = _levelData.WaterWellSettings.GetAddWaterOnClk(_levelData.WaterWellSettings.SettingsList[indexLevel]);
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
            _resourceBar.UpdateBar(Mathf.Clamp01(value));
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