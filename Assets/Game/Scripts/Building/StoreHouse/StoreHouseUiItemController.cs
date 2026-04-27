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
            _coastText.text = _storeItem.Coast.ToString("D");
            _countText.text = _storeItem.Count.ToString("D");
        }

        private void SingleButtonClk()
        {
            int countBefore = _storeItem.Count;
            _storeItem.Count = 1;
            _houseUiController.ReplaceItem(StoreItem, IsRightGroup);
            
            if (countBefore - 1 <= 0)
            {
                Destroy(gameObject);
                return;
            }
            _storeItem.Count = countBefore - 1;
        }

        private void AllButtonClk()
        {
            _houseUiController.ReplaceItem(StoreItem, IsRightGroup);
            Destroy(gameObject);
        }
    }
}