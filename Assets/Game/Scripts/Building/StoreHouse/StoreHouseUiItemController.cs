using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.Building.StoreHouse
{
    public class StoreHouseUiItemController : MonoBehaviour
    {
        public UnityAction OnUpdateValues;
        
        [Header("References")] 
        [SerializeField] private TMP_Text _coastText;
        [SerializeField] private TMP_Text _countText;

        [Space] 
        [SerializeField] private Button _singleItemLoad;
        [SerializeField] private Button _allItemLoaded;
        
        [Inject] private BuildingData _buildingData;
        private StoreHouseUiController _houseUiController;
        
        private StoreItem _storeItem;
        public StoreItem StoreItem
        {
            get => _storeItem;
            set
            {
                _storeItem = value;
                OnUpdateValues?.Invoke();
            }
        }
        
        public bool IsRightGroup { get; set; }
        
        private void Awake()
        {
            _houseUiController = _buildingData.StoreHouseUiController;

            _singleItemLoad.onClick.AddListener(SingleButtonClk);
            _allItemLoaded.onClick.AddListener(AllButtonClk);
            OnUpdateValues += UpdateValues;
        }

        private void OnDestroy()
        {
            OnUpdateValues -= UpdateValues;
            _singleItemLoad.onClick.RemoveListener(SingleButtonClk);
            _allItemLoaded.onClick.RemoveListener(AllButtonClk);
        }

        private void UpdateValues()
        {
            if (_storeItem.Count <= 0)
            {
                Destroy(gameObject);
                return;
            }
            
            _coastText.text = _storeItem.Item.Coast.ToString("D");
            _countText.text = _storeItem.Count.ToString("D");
        }

        private void SingleButtonClk()
        {
            StoreItem item = new StoreItem(_storeItem)
            {
                Count = 1
            };
            _houseUiController.ReplaceItem(item, IsRightGroup);
        }

        private void AllButtonClk()
        {
            StoreItem item = new StoreItem(_storeItem);
            _houseUiController.ReplaceItem(item, IsRightGroup);
        }
    }
}