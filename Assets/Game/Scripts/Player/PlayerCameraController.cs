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
        [Header("References Injected")] [Inject] [SerializeField]
        private CinemachineSplineDolly _splineDolly = null;
        
        [Inject] [SerializeField]
        private InputActionReference _scrollAction = null;
        
        [Header("Settings")] [SerializeField]
        private float _scrollSens = 1f;
        
        [SerializeField] [Range(0.1f, 1f)]
        private float _scrollTime = 0.5f;

        private CancellationTokenSource _scrollTokenSource = null;
        
        private void Awake()
        {
            _scrollTokenSource?.Dispose();
            _scrollTokenSource = new CancellationTokenSource();
            _scrollAction.action.performed += ChangeSplinePosition;
        }

        private void OnDestroy()
        {
            _scrollTokenSource?.Cancel();
            _scrollTokenSource?.Dispose();
            _scrollAction.action.performed -= ChangeSplinePosition;
        }

        private void ChangeSplinePosition(InputAction.CallbackContext ctx)
        {
            float scrollValue = ctx.ReadValue<Vector2>().y;
            if (scrollValue == 0)
                return;
            
            _scrollTokenSource?.Cancel();
            _scrollTokenSource?.Dispose();
            _scrollTokenSource = new CancellationTokenSource();
            
            SmoothScroll(scrollValue, _scrollTokenSource.Token).Forget();
        }

        private async UniTask SmoothScroll(float scrollValue, CancellationToken token)
        {
            float elapsedTime = 0f;
            while (!token.IsCancellationRequested && elapsedTime < _scrollTime)
            {
                _splineDolly.CameraPosition += scrollValue * _scrollSens * Time.deltaTime;
                elapsedTime += Time.deltaTime;
                await UniTask.Yield(token);
            }
        }
    }
}
