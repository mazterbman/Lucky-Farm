using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Economy;
using Game.Scripts.Mobs;
using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.Building.StoreHouse
{
    public class StoreHouseUiController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Canvas _canvas;
        [SerializeField] private AssetReference _refUiItem;
        [SerializeField] private Transform _leftParent;
        [SerializeField] private Transform _rightParent;

        [Space] 
        [SerializeField] private Button _exitButton;
        [SerializeField] private InputActionReference _exitReferences;

        [Space] 
        [SerializeField] private Button _sellItemsButtons;

        [Inject] private MobData _mobData;
        [Inject] private EconomyData _economyData;
        [Inject] private PlayerData _playerData;
        [Inject] private BuildingData _buildingData;
        [Inject] private DiContainer _diContainer;

        private List<StoreHouseUiItemController> _leftGroup = new List<StoreHouseUiItemController>();
        private List<StoreHouseUiItemController> _rightGroup = new List<StoreHouseUiItemController>();
        
        private GameObject _loadedUiItem;
        private CancellationTokenSource _tokenSource;
        private bool _refIsLoaded = false;
        private bool _refIsLoading = false;
        private bool _canSellItems = true;
        
        private void Awake()
        {
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();
            
            _exitButton.onClick.AddListener(Hide);
            _exitReferences.action.performed += Hide;
            
            _sellItemsButtons.onClick.AddListener(SellItems);
            
            LoadUiItem(_tokenSource.Token).Forget();
        }

        private void OnDestroy()
        {
            _exitButton.onClick.RemoveListener(Hide);
            _exitReferences.action.performed -= Hide;
            _sellItemsButtons.onClick.RemoveListener(SellItems);
            
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }

        public void Show(List<StoreItem> items)
        {
            _playerData.PlayerInputController.SwitchToUiMap();
            _canvas.gameObject.SetActive(true);

            foreach (var item in items)
            {
                SpawnUiItem(_tokenSource.Token, false, item).Forget();
            }
            
            UpdateSellButtonStatus();
        }

        public void Hide()
        {
            _playerData.PlayerInputController.SwitchToPlayerMap();
            ClearItems();
            _canvas.gameObject.SetActive(false);
        }

        private void Hide(InputAction.CallbackContext ctx)
        {
            Hide();
        }

        public void ReplaceItem(StoreItem item, bool isRight)
        {
            if (!_canSellItems)
                return;
            
            var groupFrom = isRight ? ref _rightGroup : ref _leftGroup;
            var groupTo = isRight ? ref _leftGroup : ref _rightGroup;
            MoveItem(item, ref groupFrom, ref groupTo, isRight);
        }
        
        public void SellItems()
        {
            if (_rightGroup.Count <= 0 || !_canSellItems)
                return;

            int countOfMoney = 0;
            foreach (var item in _rightGroup)
            {
                _mobData.MobAnimalManager.TryRemoveAnimal(item.StoreItem);
                _buildingData.StoreHouseController.TryRemoveItem(item.StoreItem);
                countOfMoney += item.StoreItem.GetAllCoast();
                //_economyData.BalanceLevelManager.TryAdd(item.StoreItem.GetAllCoast());
            }
            
            _buildingData.StoreHouseController.StartMoveTrack(countOfMoney);
            ClearGroup(_rightGroup);
            _canSellItems = false;
        }

        public void CanSellItems(bool value) => _canSellItems = value;


        private void MoveItem(StoreItem item, ref List<StoreHouseUiItemController> fromGroup, ref List<StoreHouseUiItemController> toGroup, bool isRightTarget)
        {
            UpdateCountItem(item, -1 * item.Count, ref fromGroup);
            if (ContainsStoreItem(item, ref toGroup))
            {
                UpdateCountItem(item, item.Count, ref toGroup);
            }
            else
            {
                SpawnUiItem(_tokenSource.Token, !isRightTarget, item).Forget();
            }
            
            UpdateGroup(_leftGroup, _tokenSource.Token).Forget();
            UpdateGroup(_rightGroup, _tokenSource.Token).Forget();
        }

        private void UpdateSellButtonStatus()
        {
            _sellItemsButtons.interactable = _rightGroup.Count > 0;
        }
        
        private void ClearItems()
        {
            ClearGroup(_rightGroup);
            ClearGroup(_leftGroup);
        }

        private void ClearGroup(List<StoreHouseUiItemController> group)
        {
            foreach (var value in group.Where(value => value != null))
            {
                Destroy(value.gameObject);
            }
            group.Clear();
        }

        private async UniTask UpdateGroup(List<StoreHouseUiItemController> controllers, CancellationToken token)
        {
            await UniTask.WaitForEndOfFrame(token);
            int count = controllers.Count;
            for (int i = 0; i < count; i++)
            {
                controllers[i].OnUpdateValues?.Invoke();
                if (controllers[i]) continue;
                
                controllers.RemoveAt(i);
                i--;
                count--;
            }
            
            UpdateSellButtonStatus();
        }
        
        private bool ContainsStoreItem(StoreItem item, ref List<StoreHouseUiItemController> controllersFind)
        {
            if (controllersFind.All(arg1 => arg1.StoreItem.Item.Type != item.Item.Type))
                return false;
            
            Debug.Log($"Already have this type in Group");
            return true;
        }

        private void UpdateCountItem(StoreItem item, int count, ref List<StoreHouseUiItemController> controllers)
        {
            var controller = controllers.Find(arg1 => arg1.StoreItem.Item.Type == item.Item.Type);
            controller.StoreItem.Count += count;
            controller.OnUpdateValues?.Invoke();
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
            controller.StoreItem = new StoreItem(storeItem);
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
