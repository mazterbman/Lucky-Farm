using System;
using System.Collections.Generic;
using Game.Scripts.Player;
using UnityEngine;
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
        
        [Inject] private PlayerData _playerData;
        
        private int _itemsEnabled = 0;

        private void Awake()
        {
            _buttonClose?.onClick.AddListener(Hide);
        }

        public void SetItemsEnabled(int count)
        {
            _itemsEnabled = Mathf.Clamp(count, 0, _controllers.Count - 1);
        }

        public void Show()
        {
            _canvas.gameObject.SetActive(true);
            _playerData.PlayerInputController.SwitchToUiMap();
            EnableItems();
        }

        public void Hide()
        {
            _playerData.PlayerInputController.SwitchToPlayerMap();
            OffItems();
            _canvas.gameObject.SetActive(false);
        }

        private void EnableItems()
        {
            OffItems();
            for (int i = 0; i < _itemsEnabled; i++)
            {
                _controllers[i].On();
            }
        }

        private void OffItems() => _controllers.ForEach(controller => controller.Off());   
    }
}