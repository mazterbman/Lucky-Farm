using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Scripts.Player
{
    public class PlayerInputController : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private bool _playerMapOnStart = true;
        
        [Inject] private PlayerData _playerData;

        private void Start()
        {
            _playerData.PlayerInput.SwitchCurrentActionMap(_playerMapOnStart ? StaticValues.PlayerMapName : StaticValues.UiMapName);
        }

        public void SwitchToPlayerMap()
        {
            _playerData.PlayerInput.SwitchCurrentActionMap(StaticValues.PlayerMapName);
        }

        public void SwitchToUiMap()
        {
            _playerData.PlayerInput.SwitchCurrentActionMap(StaticValues.UiMapName);
        }
    }
}
