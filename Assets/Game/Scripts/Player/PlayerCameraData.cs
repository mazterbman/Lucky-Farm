using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Scripts.Player
{
    [Serializable]
    public class PlayerCameraData
    {
        public CinemachineSplineDolly SplineDolly = null;
        public Camera Camera = null;
        
        [Space]
        public InputActionReference ScrollAction = null;
        public InputActionReference RotationActionLeft = null;
        public InputActionReference RotationActionRight = null;
        public InputActionReference PointAction = null;
    }
}