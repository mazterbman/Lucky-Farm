using System;
using Cysharp.Threading.Tasks;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.Mobs
{
    public class MobPathController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private NavMeshAgent _navMeshAgent = null;

        [Header("References Injected")]
        [Inject] [SerializeField] private MobData _mobData;
        
        [Header("Wander Settings")] 
        [SerializeField] private float _walkRadius = 10f;
        [SerializeField] private float _minWaitTime = 1f;
        [SerializeField] private float _maxWaitTime = 3f;
        
        [Header("Debug")] 
        [SerializeField] private bool _debugMode = true;
        
        private NavMeshSurface _meshSurface = null;
        private MobMovementState _currentState = MobMovementState.Wandering;
        private Vector3 _startPosition;
        private bool _isWaiting = false;
        private float _waitTimer = 0f;
        private UnityAction _onDestinationReached = null;
        
        private Vector3 _targetDestination;
        private bool _hasTarget = false;
        
        private enum MobMovementState
        {
            Wandering,      // Случайное блуждание
            MovingToTarget, // Движение к конкретной точке
            ReturningToWander // Возврат к блужданию
        }

        private void Awake()
        {
            _meshSurface = _mobData.MeshSurface;
        }

        private void Start()
        {
            UpdateWanderCenter(_meshSurface.transform.position);
            SetRandomDestination().Forget();
        }
        
        private void Update()
        {
            if (_isWaiting)
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0f)
                {
                    _isWaiting = false;
                    SetRandomDestination().Forget();
                }
                return;
            }
            
            switch (_currentState)
            {
                case MobMovementState.Wandering:
                    UpdateWanderCenter(transform.position);
                    UpdateWandering();
                    break;
                case MobMovementState.MovingToTarget:
                    UpdateMovingToTarget();
                    break;
                case MobMovementState.ReturningToWander:
                    UpdateReturningToWander();
                    break;
            }
        }
        
        #region Wandering Logic
        
        private void UpdateWandering()
        {
            if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                StartWaiting();
            }
        }
        
        private async UniTask SetRandomDestination()
        {
            Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * _walkRadius;
            randomDirection += _startPosition;
            
            if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, _walkRadius, NavMesh.AllAreas))
            {
                _navMeshAgent.SetDestination(hit.position);
                
                if (_debugMode)
                {
                    Debug.DrawLine(transform.position, hit.position, Color.green, 2f);
                }
            }
            else
            {
                await UniTask.Yield();
                await SetRandomDestination(); // Повторная попытка
            }
        }
        
        private void StartWaiting()
        {
            _isWaiting = true;
            _waitTimer = UnityEngine.Random.Range(_minWaitTime, _maxWaitTime);
        }
        
        #endregion
        
        #region Target Movement Logic
        
        private void UpdateMovingToTarget()
        {
            if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                OnTargetReached();
            }
        }
        
        private void OnTargetReached()
        {
            UpdateWanderCenter(transform.position);
            
            _onDestinationReached?.Invoke();
            _onDestinationReached = null;
            
            ReturnToWandering();
        }
        
        #endregion
        
        #region Return Logic
        
        private void UpdateReturningToWander()
        {
            if (_navMeshAgent.pathPending ||
                !(_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)) return;
            
            // Вернулись в режим блуждания
            _currentState = MobMovementState.Wandering;
            StartWaiting(); // Небольшая пауза перед следующим шагом
                
            if (_debugMode)
                Debug.Log($"[{gameObject.name}] Returned to wandering mode");
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Отправить NPC к определенной точке
        /// </summary>
        /// <param name="destination">Точка назначения</param>
        /// <param name="onReached">Колбэк при достижении цели</param>
        /// <param name="interruptWander">Прервать ли текущее блуждание</param>
        public void MoveToPoint(Vector3 destination, UnityAction onReached = null, bool interruptWander = true)
        {
            // Проверяем, находится ли точка на NavMesh
            if (!NavMesh.SamplePosition(destination, out NavMeshHit hit, 5f, NavMesh.AllAreas))
            {
                Debug.LogWarning($"Destination {destination} is not on NavMesh!", this);
                onReached?.Invoke(); // Вызываем колбэк сразу, так как цель недостижима
                return;
            }
            
            _targetDestination = hit.position;
            _onDestinationReached = onReached;
            
            // Прерываем текущие действия
            if (!interruptWander) return;
            
            _isWaiting = false;
            _currentState = MobMovementState.MovingToTarget;
            _navMeshAgent.SetDestination(_targetDestination);
                
            if (_debugMode)
            {
                Debug.Log($"[{gameObject.name}] Moving to point: {_targetDestination}");
                Debug.DrawLine(transform.position, _targetDestination, Color.red, 5f);
            }
        }
        
        /// <summary>
        /// Отправить NPC к определенному трансформу
        /// </summary>
        public void MoveToTransform(Transform target, UnityAction onReached = null, bool interruptWander = true)
        {
            if (target == null)
            {
                onReached?.Invoke();
                return;
            }
            
            MoveToPoint(target.position, onReached, interruptWander);
        }
        
        /// <summary>
        /// Вернуться к блуждающему режиму
        /// </summary>
        /// <param name="immediate">Сразу вернуться или продолжить текущее движение</param>
        public void ReturnToWandering(bool immediate = false)
        {
            if (immediate)
            {
                _currentState = MobMovementState.Wandering;
                _isWaiting = false;
                SetRandomDestination().Forget();
                
                if (_debugMode)
                    Debug.Log($"[{gameObject.name}] Immediately returned to wandering");
            }
            else
            {
                _currentState = MobMovementState.ReturningToWander;
                // Продолжаем движение к текущей цели или останавливаемся
                if (_debugMode)
                    Debug.Log($"[{gameObject.name}] Will return to wandering after reaching current target");
            }
        }
        
        public bool IsMovingToTarget => _currentState == MobMovementState.MovingToTarget;
        public bool IsWandering => _currentState == MobMovementState.Wandering;
        
        public void StopMoving()
        {
            _navMeshAgent.isStopped = true;
            _isWaiting = false;
        }
        
        public void ResumeMoving() => _navMeshAgent.isStopped = false;
        public void UpdateWanderCenter(Vector3 newCenter) => _startPosition = newCenter;
        
        #endregion
        
        #region Debug
        
        private void OnDrawGizmosSelected()
        {
            if (!_debugMode) return;
            
            Gizmos.color = Color.yellow;
            Vector3 center = Application.isPlaying ? _startPosition : transform.position;
            Gizmos.DrawWireSphere(center, _walkRadius);
                
            if (Application.isPlaying && _currentState == MobMovementState.MovingToTarget)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(_targetDestination, 0.5f);
            }
        }
        
        #endregion
    }
}