using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
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
        [SerializeField] private TMP_Text _nameText;
        [SerializeField] private Image _icoImage;

        [Space] 
        [SerializeField] private Button _singleItemLoad;
        [SerializeField] private Button _allItemLoaded;
        
        [Inject] private BuildingData _buildingData;
        
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
            _singleItemLoad.onClick.AddListener(SingleButtonClk);
            _allItemLoaded.onClick.AddListener(AllButtonClk);
            
            OnUpdateValues += UpdateValues;
            LocalizationSettings.SelectedLocaleChanged += UpdateName;
        }

        private void OnDestroy()
        {
            OnUpdateValues -= UpdateValues;
            _singleItemLoad.onClick.RemoveListener(SingleButtonClk);
            _allItemLoaded.onClick.RemoveListener(AllButtonClk);
            LocalizationSettings.SelectedLocaleChanged -= UpdateName;
        }

        private void UpdateValues()
        {
            if (_storeItem.Count <= 0)
            {
                Destroy(gameObject);
                return;
            }
            
            UpdateName(LocalizationSettings.SelectedLocale);
            _icoImage.sprite = StoreItem.Item.IcoItem;
            _coastText.text = _storeItem.Item.Coast.ToString("D");
            _countText.text = "x" + _storeItem.Count.ToString("D");
        }

        private void UpdateName(Locale locale)
        {
            _nameText.text = _storeItem.Item.LocalisationItem.GetName(locale.Identifier);
        }

        private void ReplaceItem(int count)
        {
            StoreItem item = new StoreItem(_storeItem)
            {
                Count = count
            };
            _buildingData.StoreHouseController.ReplaceItemUi(item, IsRightGroup);
        }

        private void SingleButtonClk()
        {
            ReplaceItem(1);
        }

        private void AllButtonClk()
        {
           ReplaceItem(_storeItem.Count);
        }
    }
}