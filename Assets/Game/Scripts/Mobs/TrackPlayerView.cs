using System;
using Game.Scripts.Player;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Mobs
{
    public class TrackPlayerView : MonoBehaviour
    {
        [Header("Tracking Settings")] 
        [SerializeField] private TrackMode _trackMode = TrackMode.FullLookAt;
        [SerializeField] private bool _ignoreVerticalRotation = false;
        [SerializeField] private Vector3 _rotationOffset = Vector3.zero;
        [SerializeField] private FreezeRotation _freezeRotation;
        [SerializeField] private ClampRotation _clampRotation;
        
        [Inject] private PlayerData _data = null;
        
        private Vector3 _initialEulerAngles;

        private enum TrackMode
        {
            FullLookAt,           // Полный поворот к камере (для 3D объектов)
            Billboard,            // Эффект Billboard (для UI и спрайтов)
            YAxisOnly,            // Только поворот по Y (для персонажей)
            CanvasWorld           // Специально для World Space Canvas
        }

        private void Awake()
        {
            _initialEulerAngles = transform.eulerAngles;
        }

        private void Update()
        {
            switch (_trackMode)
            {
                case TrackMode.FullLookAt: FullLookAt(); break;
                case TrackMode.Billboard: BillboardMode(); break;
                case TrackMode.YAxisOnly: YAxisOnlyMode(); break;
                case TrackMode.CanvasWorld: CanvasWorldMode(); break;
                default: break;
            }
        }

        private void FullLookAt()
        {
            Vector3 direction = _data.Camera.transform.position - transform.position;
            
            if (_ignoreVerticalRotation)
                direction.y = 0;
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                ApplyRotation(targetRotation * Quaternion.Euler(_rotationOffset));
            }
        }

        private void BillboardMode()
        {
            // Объект всегда смотрит на камеру своей передней стороной
            Vector3 direction = _data.Camera.transform.position - transform.position;
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                ApplyRotation(targetRotation * Quaternion.Euler(_rotationOffset));
            }
        }

        private void YAxisOnlyMode()
        {
            Vector3 direction = _data.Camera.transform.position - transform.position;
            direction.y = 0;
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                Quaternion finalRotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0) 
                                         * Quaternion.Euler(_rotationOffset);
                ApplyRotation(finalRotation);
            }
        }

        private void CanvasWorldMode()
        {
            Vector3 direction = _data.Camera.transform.position - transform.position;
            
            if (_ignoreVerticalRotation)
                direction.y = 0;
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                // Для Canvas нужен разворот на 180 градусов
                Quaternion finalRotation = targetRotation * Quaternion.Euler(0, 180, 0) 
                                         * Quaternion.Euler(_rotationOffset);
                ApplyRotation(finalRotation);
            }
        }
        
        private void ApplyRotation(Quaternion targetRotation)
        {
            Vector3 targetEuler = targetRotation.eulerAngles;
            Vector3 currentEuler = transform.eulerAngles;

            if (_clampRotation.UseClamp)
            {
                targetEuler.x = Mathf.Clamp(targetEuler.x, _clampRotation.ClampRotationX.x,
                    _clampRotation.ClampRotationX.y);
                targetEuler.y = Mathf.Clamp(targetEuler.y, _clampRotation.ClampRotationY.x,
                    _clampRotation.ClampRotationY.y);
                targetEuler.z = Mathf.Clamp(targetEuler.z, _clampRotation.ClampRotationZ.x,
                    _clampRotation.ClampRotationZ.y);
            }
            
            if (_freezeRotation.UseFreeze)
            {
                targetEuler.x = _freezeRotation.X ? currentEuler.x : targetEuler.x;
                targetEuler.y = _freezeRotation.Y ? currentEuler.y : targetEuler.y;
                targetEuler.z = _freezeRotation.Z ? currentEuler.z : targetEuler.z;
            }
            
            transform.eulerAngles = targetEuler;
        }
        
        [Serializable]
        private struct FreezeRotation
        {
            public bool UseFreeze;
            
            public bool X;
            public bool Y;
            public bool Z;
        }
        
        [Serializable]
        public struct ClampRotation
        {
            public bool UseClamp;
            
            [Header("Rotation Limits")]
            public Vector2 ClampRotationX; // X.min = взгляд вниз, X.max = взгляд вверх
            public Vector2 ClampRotationY; // Y.min = левый предел, Y.max = правый предел
            public Vector2 ClampRotationZ; //
        }
    }
}