using System;
using UnityEngine;

namespace Game.Scripts.Building.WaterWellController
{
    public class WaterWellUiController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private ResourceBar _resourceBar;

        private void Start()
        {
            _resourceBar.ResetBar();
        }

        public void UpdateBar(float value)
        {
            _resourceBar.UpdateBar(Mathf.Clamp01(value));
        }
    }
}