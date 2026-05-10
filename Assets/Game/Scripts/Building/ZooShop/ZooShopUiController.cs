using System;
using System.Collections.Generic;
using Game.Scripts.Economy;
using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.Building.ZooShop
{
    public class ZooShopUiController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Canvas _canvas;
        [SerializeField] private Button _buttonClose;
        
        [SerializeField] private List<ZooShopItemUiController> _controllers;

        [Inject] private BuildingData _buildingData;
        [Inject] private PlayerData _playerData;
        [Inject] private EconomyData _economyData;

        private UnityAction _onButtonClick;
        private int _itemsEnabled = 0;

        private void Awake()
        {
            _canvas.gameObject.SetActive(false);
            _buttonClose?.onClick.AddListener(Hide);

            _onButtonClick += CheckButtonStatus;
            _controllers.ForEach(controller => controller.OnClickButton = _onButtonClick);
        }

        private void OnDestroy()
        {
            _onButtonClick -= CheckButtonStatus;
            _controllers.ForEach(controller => controller.OnClickButton = null);
        }

        public void SetItemsEnabled(int count)
        {
            _itemsEnabled = Mathf.Clamp(count, 0, _controllers.Count - 1);
        }

        public void Show()
        {
            EnableItems();
            ControlPopUp(true);
        }

        public void Hide()
        {
            ControlPopUp(false);
            OffItems();
        }

        private void ControlPopUp(bool show)
        {
            _canvas.gameObject.SetActive(show);
            _economyData.BalanceLevelManager.EnableInterface(!show);
            _buildingData.StoreHouseController.EnableInterface(!show);
            
            if (show) _playerData.PlayerInputController.SwitchToUiMap();
            else _playerData.PlayerInputController.SwitchToPlayerMap();
        }

        private void EnableItems()
        {
            OffItems();
            OnItems();
        }

        private void CheckButtonStatus() => _controllers.ForEach(controller => controller.UpdateStateButton());
        private void OffItems() => _controllers.ForEach(controller => controller.Off());
        private void OnItems() => _controllers.ForEach(controller => controller.On());
    }
}