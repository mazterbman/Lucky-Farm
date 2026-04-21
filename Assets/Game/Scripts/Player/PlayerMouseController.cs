using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Game.Scripts.Player
{
    public class PlayerMouseController : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] private AssetReferenceGameObject _prefabMouse;
        [SerializeField] private Vector3 _offsetPrefab = Vector3.zero;
        
        [Space]
        [SerializeField] private float _maxRayDistance;
        [SerializeField] private LayerMask _raycastLayers;

        [Header("Debug Log")] 
        [SerializeField] [TextArea(5, 10)] private string _debugString;

        [Inject] private DiContainer _container;
        [Inject] private PlayerCameraData _playerCameraData;

        private CancellationTokenSource _tokenSource;
        private PlayerMousePrefabController _createdMouse;
        private InputActionReference _clkAction;
        private Camera _camera;

        private void Awake()
        {
            _camera = _playerCameraData.Camera;
            _clkAction = _playerCameraData.ClickAction;

            _clkAction.action.performed += OnClk;
        }

        private void Start()
        {
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();
            
            CreateMousePrefab(_tokenSource.Token).Forget();
        }

        private void Update()
        {
            if (!_createdMouse)
                return;

            if (!TryGetMouseWorldPosition(out var position))
            {
                _createdMouse.DisableObject();
                return;
            }
            
            _createdMouse.EnterGrass();
            _createdMouse.SetWorldPosition(position + _offsetPrefab);
            _debugString = position.ToString();
        }

        private void OnDestroy()
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            
            _clkAction.action.performed -= OnClk;
        }

        private async UniTask CreateMousePrefab(CancellationToken token)
        {
            if (_createdMouse != null) return;
            
            AsyncOperationHandle<GameObject> operationHandle = _prefabMouse.LoadAssetAsync<GameObject>();
    
            try
            {
                await operationHandle.Task.AsUniTask();
                if (_tokenSource.Token.IsCancellationRequested)
                    return;
                
                GameObject instance = _container.InstantiatePrefab(operationHandle.Result);
                _createdMouse = instance.GetComponent<PlayerMousePrefabController>();
            }
            finally
            {
                Addressables.Release(operationHandle);
            }
        }
        
        private bool TryGetMouseWorldPosition(out Vector3 position)
        {
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
        
            if (Physics.Raycast(ray, out RaycastHit hit, _maxRayDistance, _raycastLayers))
            {
                position = hit.point;
                return true;
            }
        
            position = Vector3.zero;
            return false;
        }

        private void OnClk(InputAction.CallbackContext ctx)
        {
            if (!_createdMouse) return;
            
            _createdMouse.ClkOnObject();
        }
    }
}
