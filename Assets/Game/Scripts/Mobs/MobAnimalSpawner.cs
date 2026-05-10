using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Building;
using Game.Scripts.Building.StoreHouse;
using Game.Scripts.Economy;
using Game.Scripts.Items;
using Game.Scripts.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Game.Scripts.Mobs
{
    public class MobAnimalSpawner : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private List<AnimalReference> _animalReferences;
        
        [Inject] private ItemsStorage _itemsStorage;
        [Inject] private BuildingData _buildingData;
        [Inject] private SettingsLevelData _levelData;
        [Inject] private MobData _mobData;
        [Inject] private EconomyData _economyData;
        [Inject] private DiContainer _diContainer;

        private List<AnimalSpawned> _animalsSpawned = new List<AnimalSpawned>();
        
        private bool _isLoaded;
        private bool _isLoading;
        private CancellationTokenSource _tokenSource;

        private void Awake()
        {
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();

            _isLoaded = false;
            _isLoading = false;
            
            LoadAssetsAsync(_tokenSource.Token).Forget();
        }

        private void OnDestroy()
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }

        
        public async UniTaskVoid TrySpawnAnimal(TypeItem typeItem, bool ignoreBalance = false)
        {
            if (!_itemsStorage.TryGetItem(typeItem, out var item))
                return;
            
            if (!ignoreBalance)
            {
                if (_economyData.BalanceLevelManager.Balance < item.Coast)
                    return;
                
                _economyData.BalanceLevelManager.TryRemove(item.Coast);
            }

            if (!_isLoaded)
            {
                if (!_isLoading) LoadAssetsAsync(_tokenSource.Token).Forget();
                await UniTask.WaitWhile(() => !_isLoaded, cancellationToken: _tokenSource.Token);
            }
            
            SpawnAsync(GetLoadedRef(typeItem), typeItem,_tokenSource.Token).Forget();
            _buildingData.StoreHouseController.TryAddItem(new StoreItem(item, 1));
        }
        
        // public async UniTaskVoid TrySpawnChicken(bool ignoreBalance = false)
        // {
        //     if (!_itemsStorage.TryGetItem(TypeItem.Chicken, out var item))
        //         return;
        //
        //     if (!ignoreBalance)
        //     {
        //         if (_economyData.BalanceLevelManager.Balance < item.Coast)
        //             return;
        //         
        //         _economyData.BalanceLevelManager.TryRemove(item.Coast);
        //     }
        //
        //     if (!_isLoaded)
        //     {
        //         if (!_isLoading) LoadAssetsAsync(_tokenSource.Token).Forget();
        //         await UniTask.WaitWhile(() => !_isLoaded, cancellationToken: _tokenSource.Token);
        //     }
        //     
        //     SpawnAsync(GetLoadedRef(TypeItem.Chicken), TypeItem.Chicken,_tokenSource.Token).Forget();
        //     _buildingData.StoreHouseController.TryAddItem(new StoreItem(item, 1));
        // }
        
        public void RemoveChicken()
        {
            if (!_animalsSpawned.Exists(arg1 => arg1.Type == TypeItem.Chicken))
                return;

            int count = _animalsSpawned.Count;
            for (int i = 0; i < count; i++)
            {
                if (_animalsSpawned[i].Type != TypeItem.Chicken) continue;
                
                Destroy(_animalsSpawned[i].Animal);
                _animalsSpawned.RemoveAt(i);
                break;
            }
        }

        private async UniTask SpawnAsync(GameObject objToSpawn, TypeItem type ,CancellationToken token)
        {
            if (!_isLoaded)
            {
                if (!_isLoading) LoadAssetsAsync(token).Forget();
                await UniTask.WaitWhile(() => !_isLoaded, cancellationToken: token);
            }
            
            GameObject GO = _diContainer.InstantiatePrefab(objToSpawn, _mobData.ParentMobs);
            GO.transform.position = _mobData.MeshSurface.center;
            _animalsSpawned.Add(new AnimalSpawned(type, GO));
        }
        
        private async UniTask LoadAssetsAsync(CancellationToken token)
        {
            if (_isLoading) 
                return;
            
            _isLoading = true;
            foreach (var animal in _animalReferences)
            {
                var handle = animal.Animal.LoadAssetAsync<GameObject>();
            
                await handle.Task.AsUniTask();
                if (token.IsCancellationRequested)
                    return;

                animal.LoadedAnimal = handle.Result;
                await UniTask.NextFrame();
            }
            
            _isLoaded = true;
        }

        private GameObject GetLoadedRef(TypeItem typeItem)
        {
            return _animalReferences.All(a => a.Type != typeItem) ?
                null :
                _animalReferences.Find(a => a.Type == typeItem).LoadedAnimal;
        }
        
        [Serializable]
        private class AnimalSpawned
        {
            public GameObject Animal;
            public TypeItem Type;

            public AnimalSpawned(TypeItem type, GameObject go)
            {
                Animal = go;
                Type = type;
            }
        }
        
        [Serializable]
        private class AnimalReference
        {
            public AssetReference Animal;
            public TypeItem Type;
            [NonSerialized] public GameObject LoadedAnimal;
        }
    }
}
