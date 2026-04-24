using System;
using Game.Scripts.Economy;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.Building
{
    public class WaterWellController : BuildingController
    {
        [Header("References")]
        [SerializeField] private ResourceBar _resourceBar;
        
        [Header("Settings")] 
        [SerializeField] private float _maxWater = 120;
        [SerializeField] [Range(0,1)] private float _addWaterOnClick = 0.35f;
        [SerializeField] private int _coastAddWater = 15;
        [SerializeField] [Range(0, 1)] private float _coastRemoveWater = 0.15f;

        [Inject] private EconomyData _economyData;

        private BalanceLevelManager _balanceLevelManager;
        private float _currentWater;
        private UnityAction<float> _onEditWaterLevel;

        private void Start()
        {
            _balanceLevelManager = _economyData.BalanceLevelManager;
            
            _currentWater = _maxWater;
            _resourceBar.ResetBar();
            _onEditWaterLevel += ChangeWaterLevel;
        }

        public override void OnClick()
        {
            if (CurrentPercentWater >= 1)
                return;
            
            if (!_balanceLevelManager.TryRemove(_coastAddWater))
                return;
            
            AddWater();
        }

        protected override void RemoveListeners()
        {
            _onEditWaterLevel -= ChangeWaterLevel;
        }

        public bool TryUseWater()
        {
            if (CurrentPercentWater < _coastRemoveWater)
                return false;

            _currentWater = Mathf.Clamp(_currentWater - _maxWater * _coastRemoveWater, 0, _maxWater);
            _onEditWaterLevel?.Invoke(CurrentPercentWater);
            return true;
        }

        private void AddWater()
        {
            float addWater = _maxWater * _addWaterOnClick;
            _currentWater = Mathf.Clamp(_currentWater + addWater, 0, _maxWater);
            Debug.Log($"Add Water = {addWater}  Percent = {CurrentPercentWater}");
            
            _onEditWaterLevel?.Invoke(CurrentPercentWater);
        }

        private void ChangeWaterLevel(float value)
        {
            _resourceBar.UpdateBar(Mathf.Clamp01(value));
        }

        public float CurrentPercentWater => Mathf.Clamp01(_currentWater / _maxWater);
    }
}