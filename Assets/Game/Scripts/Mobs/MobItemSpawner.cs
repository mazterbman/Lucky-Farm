using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;
using Random = UnityEngine.Random;

namespace Game.Scripts.Mobs
{
    public class MobItemSpawner : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private AssetReferenceGameObject _itemForSpawn;

        [Header("Settings")]
        [SerializeField] [Range(0, 600)] private float _timeForSpawn = 500;
        [SerializeField] [Range(0, 30)] private float _randomizeTimeForSpawn = 15;

        [Header("Debug")] 
        [SerializeField] [TextArea(3, 5)] private string _debugString;
        
        [Inject] private DiContainer _container;

        private GameObject _loadedItem;
        private AsyncOperationHandle<GameObject> _prefabHandle;
        private bool _isPrefabLoaded;
        
        private CancellationTokenSource _tokenSource;
        private float _currentTime;

        private void Start()
        {
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();

            LoadPrefabAsync(_tokenSource.Token).Forget();
            SpawnTimerAsync(_tokenSource.Token).Forget();
        }

        private void OnDestroy()
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }

        private async UniTask LoadPrefabAsync(CancellationToken token)
        {
            if (_itemForSpawn == null)
            {
                Debug.LogError("Item for spawn is null!");
                return;
            }
            
            if (_isPrefabLoaded)
                return;
                
            try
            {
                _debugString = "Loading prefab...";
                _prefabHandle = _itemForSpawn.LoadAssetAsync();
                await _prefabHandle.Task.AsUniTask();
                
                if (token.IsCancellationRequested)
                {
                    Addressables.Release(_prefabHandle);
                    return;
                }
                
                _loadedItem = _prefabHandle.Result;
                _isPrefabLoaded = true;
                _debugString = "Prefab loaded successfully";
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load prefab: {e}");
                _isPrefabLoaded = false;
            }
        }
        
        private async UniTask SpawnTimerAsync(CancellationToken token)
        {
            _currentTime = ApplyRandomizeTime();
            
            float time = 0;
            while (time < _currentTime && !token.IsCancellationRequested)
            {
                time += Time.deltaTime;
                _debugString = $"Time Past = {time}";
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
            
            if(token.IsCancellationRequested)
                return;
            
            SpawnItemAsync(token).Forget();
        }
        
        private async UniTask SpawnItemAsync(CancellationToken token)
        {
            if (_prefabHandle.ToUniTask(cancellationToken: token).Status == UniTaskStatus.Pending)
            {
                await _prefabHandle.ToUniTask(cancellationToken: token);
            }
            
            if (!_isPrefabLoaded || _loadedItem == null)
            {
                _debugString = "Prefab not loaded, attempting to load...";
                await LoadPrefabAsync(token);
                
                if (!_isPrefabLoaded || _loadedItem == null)
                {
                    Debug.LogError("Cannot spawn item - prefab not loaded");
                    return;
                }
            }
            
            if (token.IsCancellationRequested)
                return;

            try
            {
                GameObject GO = _container.InstantiatePrefab(_loadedItem, transform.position, Quaternion.identity, transform);
                GO.transform.parent = null;
                GO.transform.position = transform.position;
                
                _debugString = "Done Spawn";
                
                SpawnTimerAsync(token).Forget();
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to instantiate prefab: {e}");
            }
        }

        private float ApplyRandomizeTime()
        {
            var rand = Random.Range(0,2) == 0 ? Random.Range(0, _randomizeTimeForSpawn) : Random.Range(-_randomizeTimeForSpawn, 0);
            return rand + _timeForSpawn;
        }
        
    }
}
