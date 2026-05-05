using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Scripts.Player
{
    public class PlayerCameraController : MonoBehaviour
    {
        [Header("Scroll Settings")] 
        [SerializeField] private float _scrollSens = 1f;
        [SerializeField] [Range(0.1f, 1f)] private float _scrollTime = 0.5f;
        
        [Header("Rotation Settings")] 
        [SerializeField] [Range(0.1f, 1f)] private float _rotationTime = 0.5f;
        [SerializeField] private RotationSettings _rotationSettings;

        private RotateState _rotateState = RotateState.None;
        private CancellationTokenSource _scrollTokenSource = null;
        private CancellationTokenSource _rotationTokenSource = null;
        private CancellationTokenSource _rotationAsyncTokenSource = null;
        
        [Inject] private PlayerData _data = null;
        
        private enum RotateState
        {
            None,
            Left,
            Right,
        }
        
        private void Start()
        {
            _scrollTokenSource?.Dispose();
            _scrollTokenSource = new CancellationTokenSource();
            
            Transform splineTransform = _data.SplineDolly.Spline.transform;
            splineTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
            _rotateState = RotateState.None;
            
            _data.SplineDolly.CameraPosition = 0f;
            _data.ScrollAction.action.performed += ChangeSplinePosition;
            _data.RotationAction.action.started += RotatePressed;
            _data.RotationAction.action.canceled += RotationCanceled;
        }

        private void OnDestroy()
        {
            _scrollTokenSource?.Cancel();
            _scrollTokenSource?.Dispose();
            
            _rotationTokenSource?.Cancel();
            _rotationTokenSource?.Dispose();
            
            _rotationAsyncTokenSource?.Cancel();
            _rotationAsyncTokenSource?.Dispose();
            
            _rotateState = RotateState.None;
            
            _data.ScrollAction.action.performed -= ChangeSplinePosition;
            _data.RotationAction.action.started -= RotatePressed;
            _data.RotationAction.action.canceled -= RotationCanceled;
        }

        private void ChangeSplinePosition(InputAction.CallbackContext ctx)
        {
            float scrollValue = ctx.ReadValue<Vector2>().y;
            if (scrollValue == 0)
                return;
            
            _scrollTokenSource?.Cancel();
            _scrollTokenSource?.Dispose();
            _scrollTokenSource = new CancellationTokenSource();
            
            SmoothScroll(scrollValue).Forget();
        }

        private async UniTask SmoothScroll(float scrollValue)
        {
            float elapsedTime = 0f;
            while (!_scrollTokenSource.IsCancellationRequested && elapsedTime < _scrollTime)
            {
                _data.SplineDolly.CameraPosition += scrollValue * _scrollSens * Time.deltaTime;
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(_scrollTokenSource.Token);
            }
        }

        private void RotatePressed(InputAction.CallbackContext ctx)
        {
            if (_rotationAsyncTokenSource != null)
            {
                _rotationAsyncTokenSource.Cancel();
                _rotationAsyncTokenSource.Dispose();
            }

            _rotationAsyncTokenSource = new CancellationTokenSource();
            RotationAsync(_rotationAsyncTokenSource.Token).Forget();
        }

        private void RotationCanceled(InputAction.CallbackContext ctx)
        {
            if (_rotationAsyncTokenSource == null) return;
            
            _rotationAsyncTokenSource.Cancel();
        }

        private async UniTask RotationAsync(CancellationToken token)
        {
            Vector2 lastPos = Mouse.current.position.ReadValue();
    
            while (!token.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                Vector2 currentPos = Mouse.current.position.ReadValue();
                Vector2 delta = currentPos - lastPos;
        
                delta *= _rotationSettings.Sensitivity / 100;
                
                Vector3 currentRotation = _data.CameraRotationTarget.localEulerAngles;
                
                currentRotation.x = NormalizeAngle(currentRotation.x);
                currentRotation.y = NormalizeAngle(currentRotation.y);
                
                currentRotation.y += delta.x;
                currentRotation.x -= delta.y;
                
                currentRotation.x = Mathf.Clamp(currentRotation.x, _rotationSettings.ClampRotationX.x, _rotationSettings.ClampRotationX.y);
                currentRotation.y = Mathf.Clamp(currentRotation.y, _rotationSettings.ClampRotationY.x, _rotationSettings.ClampRotationY.y);
                currentRotation.z = 0f;
                
                _data.CameraRotationTarget.localRotation = Quaternion.Euler(currentRotation);
        
                lastPos = currentPos;
            }
        }

        private float NormalizeAngle(float angle)
        {
            angle %= 360f;
            if (angle > 180f) angle -= 360f;
            if (angle < -180f) angle += 360f;
            return angle;
        }
        
        [Serializable]
        private struct RotationSettings
        {
            [Header("Rotation Limits")]
            public Vector2 ClampRotationX; // X.min = взгляд вниз, X.max = взгляд вверх
            public Vector2 ClampRotationY; // Y.min = левый предел, Y.max = правый предел
    
            [Header("Mouse Settings")]
            [Range(1f, 100f)] public float Sensitivity;
        }
    }
}
