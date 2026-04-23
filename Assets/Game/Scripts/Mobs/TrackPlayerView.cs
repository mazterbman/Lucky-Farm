using System;
using Game.Scripts.Player;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Mobs
{
    public class TrackPlayerView : MonoBehaviour
    {
        [Header("References Injected")] 
        [Inject] [SerializeField] private PlayerCameraData _cameraData = null;

        [Header("Tracking Settings")] 
        [SerializeField] private TrackMode _trackMode = TrackMode.FullLookAt;
        [SerializeField] private bool _ignoreVerticalRotation = false;
        [SerializeField] private Vector3 _rotationOffset = Vector3.zero;
        [SerializeField] private FreezeRotation _freezeRotation;
        
        private Camera _camera = null;
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
            _camera = _cameraData.Camera;
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
            Vector3 direction = _camera.transform.position - transform.position;
            
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
            Vector3 direction = _camera.transform.position - transform.position;
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                ApplyRotation(targetRotation * Quaternion.Euler(_rotationOffset));
            }
        }

        private void YAxisOnlyMode()
        {
            Vector3 direction = _camera.transform.position - transform.position;
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
            Vector3 direction = _camera.transform.position - transform.position;
            
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
            
            // Применяем заморозку осей
            if (_freezeRotation.X)
                targetEuler.x = currentEuler.x;
            
            if (_freezeRotation.Y)
                targetEuler.y = currentEuler.y;
            
            if (_freezeRotation.Z)
                targetEuler.z = currentEuler.z;
            
            transform.eulerAngles = targetEuler;
        }
        
        [Serializable]
        private struct FreezeRotation
        {
            public bool X;
            public bool Y;
            public bool Z;
        }
    }
}