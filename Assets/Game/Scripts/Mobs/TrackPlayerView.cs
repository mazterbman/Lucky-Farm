using System;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Mobs
{
    public class TrackPlayerView : MonoBehaviour
    {
        [Header("References Injected")] [Inject] [SerializeField]
        private Camera _camera = null;

        [Header("Tracking Settings")] [SerializeField] 
        private TrackMode _trackMode = TrackMode.FullLookAt;
        
        [SerializeField]
        private bool _ignoreVerticalRotation = false;
        
        [SerializeField]
        private Vector3 _rotationOffset = Vector3.zero;

        private enum TrackMode
        {
            FullLookAt,           // Полный поворот к камере (для 3D объектов)
            Billboard,            // Эффект Billboard (для UI и спрайтов)
            YAxisOnly,            // Только поворот по Y (для персонажей)
            CanvasWorld           // Специально для World Space Canvas
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
                transform.rotation = targetRotation * Quaternion.Euler(_rotationOffset);
            }
        }

        private void BillboardMode()
        {
            // Объект всегда смотрит на камеру своей передней стороной
            Vector3 direction = _camera.transform.position - transform.position;
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = targetRotation * Quaternion.Euler(_rotationOffset);
            }
        }

        private void YAxisOnlyMode()
        {
            Vector3 direction = _camera.transform.position - transform.position;
            direction.y = 0;
            
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0) 
                                    * Quaternion.Euler(_rotationOffset);
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
                transform.rotation = targetRotation * Quaternion.Euler(0, 180, 0) 
                                    * Quaternion.Euler(_rotationOffset);
            }
        }
    }
}
