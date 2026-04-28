using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Player
{
    [Serializable]
    public class PlayerData
    {
        public CinemachineSplineDolly SplineDolly = null;
        public Camera Camera = null;
        public PlayerInput PlayerInput;
        public PlayerInputController PlayerInputController;
        
        [Space]
        public InputActionReference ScrollAction = null;
        public InputActionReference RotationActionLeft = null;
        public InputActionReference RotationActionRight = null;
        public InputActionReference ClickAction;
    }
}