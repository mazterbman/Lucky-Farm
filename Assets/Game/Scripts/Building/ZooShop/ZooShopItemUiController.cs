using System;
using Game.Scripts.Economy;
using Game.Scripts.Items;
using Game.Scripts.Mobs;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game.Scripts.Building.ZooShop
{
    public class ZooShopItemUiController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Button _button;
        [SerializeField] private CanvasGroup _canvasGroup;

        [Header("Settings")] 
        [SerializeField] private Item _itemBuy;

        [Inject] private EconomyData _economyData;
        [Inject] private MobData _mobData;

        private void Awake()
        {
            _button?.onClick.AddListener(OnClick);
        }

        private void OnDestroy()
        {
            _button?.onClick.RemoveListener(OnClick);
        }

        public void Off()
        {
            _button.interactable = false;
            _canvasGroup.alpha = 1;
            _canvasGroup.gameObject.SetActive(true);
        }

        public void On()
        {
            _button.interactable = true;
            _canvasGroup.alpha = 0;
            _canvasGroup.gameObject.SetActive(false);
            
            UpdateStateButton();
        }

        private void OnClick()
        {
            switch (_itemBuy.Type)
            {
                case TypeItem.Chicken:
                    _mobData.MobAnimalSpawner.TrySpawnChicken().Forget();
                    break;
                
                case TypeItem.Egg:
                case TypeItem.Omelet:
                default: break;
            }
            
            UpdateStateButton();
        }

        private void UpdateStateButton()
        {
            _button.interactable = _economyData.BalanceLevelManager.Balance >= _itemBuy.Coast;
        }
    }
}