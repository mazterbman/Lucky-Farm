using System;
using Game.Scripts.Settings;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Building.ZooShop
{
    public class ZooShopController : BuildingController
    {
        [Header("References")]
        [SerializeField] private ZooShopUiController _uiController;

        [Inject] private SettingsLevelData _levelData;

        public override void OnClick()
        {
            OpenMenu();
        }

        protected override void RemoveListeners()
        {
        }

        protected override void LoadSettings(int level)
        {
            int count = _levelData.ZooShopSettings.GetItemsEnabled(_levelData.ZooShopSettings.Levels[level - 1]);
            _uiController.SetItemsEnabled(count);
        }
        
        private void OpenMenu()
        {
            _uiController.Show();
        }
    }
}
