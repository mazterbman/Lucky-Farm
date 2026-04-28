using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Economy;
using Game.Scripts.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.Mobs
{
    public class MobAnimalSpawner : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private AssetReference _chickenPrefab;

        [Space] 
        [SerializeField] private Button _buttonSpawnChicken;

        [Inject] private SettingsLevelData _levelData;
        [Inject] private MobData _mobData;
        [Inject] private EconomyData _economyData;
        [Inject] private DiContainer _diContainer;

        private bool _isLoaded;
        private bool _isLoading;
        private GameObject _loadedChicken;
        private CancellationTokenSource _tokenSource;

        private void Awake()
        {
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();

            _isLoaded = false;
            _isLoading = false;
            
            _buttonSpawnChicken.onClick.AddListener(SpawnChicken);
            _economyData.BalanceLevelManager.OnUpdateBalance += UpdateButtonsState;
            
            LoadAssetsAsync(_tokenSource.Token).Forget();
        }

        private void OnDestroy()
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            
            _buttonSpawnChicken.onClick.RemoveListener(SpawnChicken);
            _economyData.BalanceLevelManager.OnUpdateBalance -= UpdateButtonsState;
        }

        public void SpawnChicken()
        {
            if (_economyData.BalanceLevelManager.Balance < _levelData.ChickenCoast)
                return;
            
            SpawnAsync(_loadedChicken, _tokenSource.Token).Forget();
            _economyData.BalanceLevelManager.TryRemove(_levelData.ChickenCoast);
        }

        private void UpdateButtonsState()
        {
            if (_buttonSpawnChicken)
                _buttonSpawnChicken.interactable = _economyData.BalanceLevelManager.Balance >= _levelData.ChickenCoast;
        }

        private async UniTask SpawnAsync(GameObject objToSpawn, CancellationToken token)
        {
            if (!_isLoaded)
            {
                if (!_isLoading) LoadAssetsAsync(token).Forget();
                await UniTask.WaitWhile(() => !_isLoaded, cancellationToken: token);
            }

            GameObject GO = _diContainer.InstantiatePrefab(objToSpawn, _mobData.ParentMobs);
            GO.transform.position = _mobData.MeshSurface.center;
        }
        
        private async UniTask LoadAssetsAsync(CancellationToken token)
        {
            _isLoading = true;
            var handle = _chickenPrefab.LoadAssetAsync<GameObject>();
            
            await handle.Task.AsUniTask();
            if (token.IsCancellationRequested)
                return;

            _loadedChicken = handle.Result;
            _isLoaded = true;
        } 
    }
}
