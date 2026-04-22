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
        [Header("References Injected")] 
        [Inject] [SerializeField] private PlayerCameraData _cameraData = null;
        
        [Header("Scroll Settings")] 
        [SerializeField] private float _scrollSens = 1f;
        [SerializeField] [Range(0.1f, 1f)] private float _scrollTime = 0.5f;
        
        [Header("Rotation Settings")] 
        [SerializeField] [Range(0.1f, 1f)] private float _rotationTime = 0.5f;

        private RotateState _rotateState = RotateState.None;
        private CancellationTokenSource _scrollTokenSource = null;
        private CancellationTokenSource _rotationTokenSource = null;
        private CinemachineSplineDolly _splineDolly = null;
        private InputActionReference _scrollAction = null;
        private InputActionReference _rotationActionLeft = null;
        private InputActionReference _rotationActionRight = null;
        
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
            
            _splineDolly = _cameraData.SplineDolly;
            _scrollAction = _cameraData.ScrollAction;
            _rotationActionLeft = _cameraData.RotationActionLeft;
            _rotationActionRight = _cameraData.RotationActionRight;
            
            Transform splineTransform = _cameraData.SplineDolly.Spline.transform;
            splineTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
            _rotateState = RotateState.None;
            
            _splineDolly.CameraPosition = 0f;
            _scrollAction.action.performed += ChangeSplinePosition;
            _rotationActionLeft.action.performed += RotateCameraLeft;
            _rotationActionRight.action.performed += RotateCameraRight;
        }

        private void OnDestroy()
        {
            _scrollTokenSource?.Cancel();
            _scrollTokenSource?.Dispose();
            
            _rotationTokenSource?.Cancel();
            _rotationTokenSource?.Dispose();
            
            _rotateState = RotateState.None;
            
            _scrollAction.action.performed -= ChangeSplinePosition;
            _rotationActionLeft.action.performed -= RotateCameraLeft;
            _rotationActionRight.action.performed -= RotateCameraRight;
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
                _splineDolly.CameraPosition += scrollValue * _scrollSens * Time.deltaTime;
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(_scrollTokenSource.Token);
            }
        }
        
        private void RotateCameraRight(InputAction.CallbackContext obj)
        {
            if (_rotateState == RotateState.Right)
                return;
            
            _rotateState = RotateState.Right;
            _rotationTokenSource?.Cancel();
            _rotationTokenSource?.Dispose();
            _rotationTokenSource = new CancellationTokenSource();
            
            Debug.Log("Rotate Right");
            RotateCamera(true).Forget();
        }

        private void RotateCameraLeft(InputAction.CallbackContext obj)
        {
            if (_rotateState == RotateState.Left)
                return;
            
            _rotateState = RotateState.Left;
            _rotationTokenSource?.Cancel();
            _rotationTokenSource?.Dispose();
            _rotationTokenSource = new CancellationTokenSource();
            
            Debug.Log("Rotate Left");
            RotateCamera(false).Forget();
        }
        
        private async UniTask RotateCamera(bool isRight)
        {
            Transform splineTransform = _cameraData.SplineDolly.Spline.transform;
            float currentAngle = splineTransform.eulerAngles.y;
            currentAngle = Mathf.Round(currentAngle / 90f) * 90f;
            
            float targetAngle = isRight ? currentAngle - 90f : currentAngle + 90f;
            targetAngle = ((targetAngle % 360f) + 360f) % 360f;
            
            Quaternion rotationStart = splineTransform.rotation;
            Quaternion rotationEnd = Quaternion.Euler(0f, targetAngle, 0f);
            
            float elapsedTime = 0f;
            while (!_rotationTokenSource.IsCancellationRequested && elapsedTime < _rotationTime)
            {
                float t = elapsedTime / _rotationTime;
                Quaternion rotation = Quaternion.Slerp(rotationStart, rotationEnd, t);
                splineTransform.rotation = rotation;
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(_rotationTokenSource.Token);
            }
            
            _rotateState = RotateState.None;
        }
    }
}
