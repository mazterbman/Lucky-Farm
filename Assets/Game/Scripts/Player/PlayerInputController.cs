using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.Scripts.Player
{
    public class PlayerInputController : MonoBehaviour
    {
        public UnityAction OnSwitchPlayerMap;
        public UnityAction OnSwitchUiMap;
        
        [Header("References")] 
        [SerializeField] private PlayerInput _playerInput;
        
        [Header("Settings")] 
        [SerializeField] private bool _playerMapOnStart = true;

        private void Start()
        {
            _playerInput.SwitchCurrentActionMap(_playerMapOnStart ? StaticValues.PlayerMapName : StaticValues.UiMapName);
        }

        public void SwitchToPlayerMap()
        {
            _playerInput.SwitchCurrentActionMap(StaticValues.PlayerMapName);
            OnSwitchPlayerMap?.Invoke();
        }

        public void SwitchToUiMap()
        {
            _playerInput.SwitchCurrentActionMap(StaticValues.UiMapName);
            OnSwitchUiMap?.Invoke();
        }
    }
}
