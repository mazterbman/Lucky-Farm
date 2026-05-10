using System;
using Game.Scripts.Economy;
using Game.Scripts.Items;
using Game.Scripts.Mobs;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.Building.ZooShop
{
    public class ZooShopItemUiController : MonoBehaviour
    {
        public UnityAction OnClickButton;
        
        [Header("References")] 
        [SerializeField] private Button _button;
        [SerializeField] private DarkerUiController _darkerUiController;

        [Space] 
        [SerializeField] private TMP_Text _nameZoo;
        [SerializeField] private TMP_Text _coastZoo;
        [SerializeField] private TMP_Text _countZoo;
        [SerializeField] private Image _icoImage;

        [Header("Settings")] 
        [SerializeField] private Item _itemBuy;

        [Inject] private EconomyData _economyData;
        [Inject] private MobData _mobData;
        [Inject] private BuildingData _buildingData;

        private void Awake()
        {
            _button?.onClick.AddListener(OnClick);

            _icoImage.sprite = _itemBuy.IcoItem;
            UpdateText(LocalizationSettings.SelectedLocale);

            LocalizationSettings.SelectedLocaleChanged += UpdateText;
        }

        private void OnDestroy()
        {
            _button?.onClick.RemoveListener(OnClick);
            LocalizationSettings.SelectedLocaleChanged -= UpdateText;
        }

        public void Off()
        {
            _button.interactable = false;
            _darkerUiController.ChangeColor(false);
        }

        public void On()
        {
            _button.interactable = true;
            _darkerUiController.ChangeColor(true);
            
            if (!_buildingData.StoreHouseController.TryGetCountItem(_itemBuy.Type, out var count))
                return;

            _countZoo.text = "x" + count;
            UpdateStateButton();
        }
        
        public void UpdateStateButton()
        {
            bool goodState = _economyData.BalanceLevelManager.Balance >= _itemBuy.Coast;
            _button.interactable = goodState;
            _darkerUiController.ChangeColor(goodState);
        }
        

        private void OnClick()
        {
            _mobData.MobAnimalSpawner.TrySpawnAnimal(_itemBuy.Type).Forget();
            OnClickButton?.Invoke();
        }

        private void UpdateText(Locale locale)
        {
            _nameZoo.text = _itemBuy.LocalisationItem.GetName(locale.Identifier);
        }
    }
}