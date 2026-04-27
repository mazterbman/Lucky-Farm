using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

namespace Game.Scripts.Building.StoreHouse
{
    public class StoreHouseUiController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AssetReference _refUiItem;
        [SerializeField] private Transform _leftParent;
        [SerializeField] private Transform _rightParent;

        [Inject] private DiContainer _diContainer;

        private List<StoreHouseUiItemController> _leftGroup = new List<StoreHouseUiItemController>();
        private List<StoreHouseUiItemController> _rightGroup = new List<StoreHouseUiItemController>();
        
        private GameObject _loadedUiItem;
        private CancellationTokenSource _tokenSource;
        private bool _refIsLoaded = false;
        private bool _refIsLoading = false;
        
        private void Awake()
        {
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();
            
            LoadUiItem(_tokenSource.Token).Forget();
        }

        private void OnDestroy()
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }

        public void Show(List<StoreItem> items)
        {
            foreach (var value in _leftGroup)
            {
                value.OnUpdateValues?.Invoke();
            }
            foreach (var value in _rightGroup)
            {
                value.OnUpdateValues?.Invoke();
            }
            
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
        
        public void SpawnItemLeft(StoreItem item)
        {
            if (ContainsStoreItem(item, ref _leftGroup)) return;
            SpawnUiItem(_tokenSource.Token, false, item).Forget();
        }

        public void SpawnItemRight(StoreItem item)
        {
            if (ContainsStoreItem(item, ref _rightGroup)) return;
            SpawnUiItem(_tokenSource.Token, true, item).Forget();
        }

        public void ReplaceItem(StoreItem item, bool isRight)
        {
            if (isRight)
            {
                SpawnItemRight(item);
            }
            else
            {
                SpawnItemLeft(item);
            }
        }
        
        
        private bool ContainsStoreItem(StoreItem item, ref List<StoreHouseUiItemController> controllers)
        {
            if (controllers.All(arg1 => arg1.StoreItem.Type != item.Type))
                return false;
            
            Debug.Log($"Already have this type in Group");
            var controller = controllers.Find(arg1 => arg1.StoreItem.Type == item.Type);
            controller.StoreItem.Count += item.Count;
            controller.OnUpdateValues?.Invoke();
            return true;
        }

        private async UniTask SpawnUiItem(CancellationToken token, bool isRight, StoreItem storeItem)
        {
            if (!_refIsLoaded)
            {
                if (_refIsLoading)
                    await UniTask.WaitWhile(() => !_refIsLoaded, cancellationToken: token);
                else
                {
                    await LoadUiItem(token);
                }
            }

            GameObject GO = _diContainer.InstantiatePrefab(_loadedUiItem, isRight ? _rightParent : _leftParent);
            GO.name = $"Object + {GO.transform.GetSiblingIndex()}";
            Debug.Log($"Was Created Obj = {GO.name} on Parent = {GO.transform.parent.name}");

            StoreHouseUiItemController controller = GO.GetComponent<StoreHouseUiItemController>();
            controller.StoreItem = storeItem;
            controller.IsRightGroup = isRight;
            
            if (isRight) _rightGroup.Add(controller);
            else _leftGroup.Add(controller);
        }
        
        private async UniTask LoadUiItem(CancellationToken token)
        {
            if (_refIsLoaded || _refIsLoading)
                return;

            try
            {
                _refIsLoading = true;

                var handle = _refUiItem.LoadAssetAsync<GameObject>().Task;
                await handle.AsUniTask();
                if (token.IsCancellationRequested)
                {
                    Addressables.Release(_refUiItem);
                    return;
                }
            
                _loadedUiItem = handle.Result;
            
                _refIsLoaded = true;
            }
            catch (Exception e)
            {
                _refIsLoaded = false;
                _refIsLoading = false;
                Addressables.Release(_refUiItem);
            }
            
            Debug.Log("Was Loaded Obj");
        } 
    }
}
