using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Scripts.Player
{
    public class PlayerMouseController : MonoBehaviour
    {
        [Inject] [SerializeField] private PlayerCameraData _playerCameraData;

        [Header("Settings")] 
        [SerializeField] private GameObject _prefabMouse;
        [SerializeField] private Vector3 _offsetPrefab = Vector3.zero;
        
        [Space]
        [SerializeField] private float _maxRayDistance;
        [SerializeField] private LayerMask _raycastLayers;

        [Header("Debug Log")] 
        [SerializeField] [TextArea(5, 10)] private string _debugString;

        private PlayerMousePrefabController _createdMouse;
        private Camera _camera;
        private InputActionReference _pointAction;
        private Vector2 _mousePosition;

        private void Awake()
        {
            _camera = _playerCameraData.Camera;
            _pointAction = _playerCameraData.PointAction;
        }

        private void Start()
        {
            _pointAction.action.performed += LookPerformed;
            CreateMousePrefab();
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
            
            _createdMouse.EnableObject();
            _createdMouse.SetWorldPosition(position + _offsetPrefab);
            _debugString = position.ToString();
        }

        private void OnDestroy()
        {
            _pointAction.action.performed -= LookPerformed;
        }

        private void LookPerformed(InputAction.CallbackContext ctx)
        {
            _mousePosition = ctx.ReadValue<Vector2>();
        }

        private void CreateMousePrefab()
        {
            if (_createdMouse) return;

            _createdMouse = Instantiate(_prefabMouse).GetComponent<PlayerMousePrefabController>();
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
    }
}
