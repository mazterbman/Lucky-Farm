using System;
using UnityEngine;

namespace Game.Scripts.Building
{
    public class WaterWellController : BuildingController
    {
        [Header("Settings")] 
        [SerializeField] private float _maxWater = 120;
        [SerializeField] [Range(0,1)] private float _addWaterOnClick = 0.35f;
        [SerializeField] private int _coastAddWater = 15;
        [SerializeField] [Range(0, 1)] private float _coastRemoveWater = 0.15f;

        private float _currentWater;

        private void Start()
        {
            _currentWater = _maxWater;
        }

        public override void OnClick()
        {
            //ToDo make Check balance 
            if (CurrentPercentWater >= 1)
                return;
            
            AddWater();
        }
        
        public bool TryUseWater()
        {
            if (CurrentPercentWater < _coastRemoveWater)
                return false;

            _currentWater = Mathf.Clamp(_currentWater - _maxWater * _coastRemoveWater, 0, _maxWater);
            return true;
        }

        private void AddWater()
        {
            float addWater = _maxWater * _addWaterOnClick;
            _currentWater = Mathf.Clamp(_currentWater + addWater, 0, _maxWater);
            Debug.Log($"Add Water = {addWater}  Percent = {CurrentPercentWater}");
        }

        public float CurrentPercentWater => Mathf.Clamp01(_currentWater / _maxWater);
    }
}