using System;
using Game.Scripts.Economy;
using Game.Scripts.Items;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.Mobs
{
    public class MobAnimalSpawnerUI : MonoBehaviour
    {
        [Space] 
        [SerializeField] private Button _buttonSpawnChicken;

        [Inject] private ItemsStorage _itemsStorage;
        [Inject] private MobData _mobData;
        [Inject] private EconomyData _economyData;
        
        private void Awake()
        {
            _buttonSpawnChicken.onClick.AddListener(SpawnChicken);
            _economyData.BalanceLevelManager.OnUpdateBalance += UpdateButtonsState;
        }

        private void OnDestroy()
        {
            _buttonSpawnChicken.onClick.RemoveListener(SpawnChicken);
            _economyData.BalanceLevelManager.OnUpdateBalance -= UpdateButtonsState;
        }
        
        private void UpdateButtonsState()
        {
            if (!_itemsStorage.TryGetItem(TypeItem.Chicken, out var item))
                return;
            
            if (_buttonSpawnChicken)
                _buttonSpawnChicken.interactable = _economyData.BalanceLevelManager.Balance >= item.Coast;
        }

        private void SpawnChicken()
        {
            Debug.Log("TrySpawn");
            _mobData.MobAnimalSpawner.SpawnChicken().Forget();
        }
    }
}