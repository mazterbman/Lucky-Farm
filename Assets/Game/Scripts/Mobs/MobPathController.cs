using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Game.Scripts.Mobs
{
    public class MobPathController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private NavMeshAgent _navMeshAgent = null;
        
        [Header("References Injected")] 
        [Inject] [SerializeField] private NavMeshSurface _meshSurface = null;
        
        [Header("Wander Settings")] 
        [SerializeField] private float _walkRadius = 10f;
        [SerializeField] private float _minWaitTime = 1f;
        [SerializeField] private float _maxWaitTime = 3f;
        
        [Header("Debug")] 
        [SerializeField] private bool _debugMode = true;
        
        private MobMovementState _currentState = MobMovementState.Wandering;
        private Vector3 _startPosition;
        private bool _isWaiting = false;
        private float _waitTimer = 0f;
        private Action _onDestinationReached = null;
        
        // Для хранения точки назначения
        private Vector3 _targetDestination;
        private bool _hasTarget = false;
        
        private enum MobMovementState
        {
            Wandering,      // Случайное блуждание
            MovingToTarget, // Движение к конкретной точке
            ReturningToWander // Возврат к блужданию
        }
        
        private void Start()
        {
            _startPosition = transform.position;
            SetRandomDestination();
        }
        
        private void Update()
        {
            switch (_currentState)
            {
                case MobMovementState.Wandering:
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
            if (_isWaiting)
            {
                _waitTimer -= Time.deltaTime;
                if (_waitTimer <= 0f)
                {
                    _isWaiting = false;
                    SetRandomDestination();
                }
                return;
            }
            
            if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                StartWaiting();
            }
        }
        
        private void SetRandomDestination()
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
                SetRandomDestination(); // Повторная попытка
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
                // Достигли цели
                OnTargetReached();
            }
        }
        
        private void OnTargetReached()
        {
            _onDestinationReached?.Invoke();
            _onDestinationReached = null;
            
            // Возвращаемся к блужданию
            ReturnToWandering();
        }
        
        #endregion
        
        #region Return Logic
        
        private void UpdateReturningToWander()
        {
            if (!_navMeshAgent.pathPending && _navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                // Вернулись в режим блуждания
                _currentState = MobMovementState.Wandering;
                StartWaiting(); // Небольшая пауза перед следующим шагом
                
                if (_debugMode)
                    Debug.Log($"[{gameObject.name}] Returned to wandering mode");
            }
        }
        
        #endregion
        
        #region Public API
        
        /// <summary>
        /// Отправить NPC к определенной точке
        /// </summary>
        /// <param name="destination">Точка назначения</param>
        /// <param name="onReached">Колбэк при достижении цели</param>
        /// <param name="interruptWander">Прервать ли текущее блуждание</param>
        public void MoveToPoint(Vector3 destination, Action onReached = null, bool interruptWander = true)
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
            if (interruptWander)
            {
                _isWaiting = false;
                _currentState = MobMovementState.MovingToTarget;
                _navMeshAgent.SetDestination(_targetDestination);
                
                if (_debugMode)
                {
                    Debug.Log($"[{gameObject.name}] Moving to point: {_targetDestination}");
                    Debug.DrawLine(transform.position, _targetDestination, Color.red, 5f);
                }
            }
        }
        
        /// <summary>
        /// Отправить NPC к определенному трансформу
        /// </summary>
        public void MoveToTransform(Transform target, Action onReached = null, bool interruptWander = true)
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
                SetRandomDestination();
                
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
        
        /// <summary>
        /// Получить текущий статус движения
        /// </summary>
        public bool IsMovingToTarget => _currentState == MobMovementState.MovingToTarget;
        public bool IsWandering => _currentState == MobMovementState.Wandering;
        
        /// <summary>
        /// Остановить NPC и отменить все действия
        /// </summary>
        public void StopMoving()
        {
            _navMeshAgent.isStopped = true;
            _isWaiting = false;
        }
        
        /// <summary>
        /// Возобновить движение
        /// </summary>
        public void ResumeMoving()
        {
            _navMeshAgent.isStopped = false;
        }
        
        /// <summary>
        /// Обновить центр блуждания
        /// </summary>
        public void UpdateWanderCenter(Vector3 newCenter)
        {
            _startPosition = newCenter;
        }
        
        #endregion
        
        #region Debug
        
        private void OnDrawGizmosSelected()
        {
            if (_debugMode)
            {
                Gizmos.color = Color.yellow;
                Vector3 center = Application.isPlaying ? _startPosition : transform.position;
                Gizmos.DrawWireSphere(center, _walkRadius);
                
                if (Application.isPlaying && _currentState == MobMovementState.MovingToTarget)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere(_targetDestination, 0.5f);
                }
            }
        }
        
        #endregion
    }
}