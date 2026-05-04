using System;
using UnityEngine;

namespace Game.Scripts.Building.SweetShop
{
    public class SweetShopUiController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ResourceBar _resourceBar;

        private void Start()
        {
            _resourceBar.ResetBar();
            _resourceBar.UpdateBar(0);
        }

        public void UpdateBar(float value)
        {
            _resourceBar.UpdateBar(value);
        }
    }
}