using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Game.Scripts.Grass
{
    public class GrassManager : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private ColliderListener _colliderListener;
        
        [Header("Settings")] 
        [SerializeField] [Range(0,5)] private float _minDistanceToSpawn;
        [SerializeField] private float _cellSize = 2f;
        
        [Header("Debug Information")] 
        [SerializeField] private List<GrassController> _controllers;

        [Inject] private DiContainer _container;
        [Inject] private GrassData _grassData;
        
        private CancellationTokenSource _tokenSource;
        private Transform _grassParent;
        private AssetReferenceGameObject _grassPrefab;
        private Dictionary<Vector2Int, HashSet<GrassController>> _grid;

        private void Start()
        {
            _grassPrefab = _grassData.GrassPrefab;
            _grassParent = _grassData.Parent;
            _grid = new Dictionary<Vector2Int, HashSet<GrassController>>();

            _colliderListener.OnTriggerStayAction += GrassStay;
            
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _colliderListener.OnTriggerStayAction -= GrassStay;
            
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }

        public async UniTaskVoid CreateGrass(Vector3 worldPosition)
        {
            if (!CanSpawnAtPosition(worldPosition))
                return;
            
            AsyncOperationHandle<GameObject> operationHandle = _grassPrefab.LoadAssetAsync<GameObject>();
            try
            {
                await operationHandle.Task.AsUniTask();
                if (_tokenSource.Token.IsCancellationRequested)
                    return;
                
                GameObject instance = _container.InstantiatePrefab(operationHandle.Result);
                instance.transform.parent = _grassParent;
                instance.transform.position = worldPosition;
                
                GrassController controller = instance.GetComponent<GrassController>();
                _controllers.Add(controller);
                
                AddToGrid(controller);
            }
            finally
            {
                Addressables.Release(operationHandle);
            }
        }
        
         public void RemoveGrass(GrassController controller)
        {
            if (controller == null) return;
            
            RemoveFromGrid(controller);
            _controllers.Remove(controller);
            controller.Remove();
        }

        private void GrassStay(Collider other)
        {
            if (!other.CompareTag(StaticValues.MouseTag))
                return;

            PlayerMousePrefabController controller = other.GetComponent<PlayerMousePrefabController>();
            controller.OnGrass();
        }
         
        private bool CanSpawnAtPosition(Vector3 positionSpawn)
        {
            Vector2Int centerCell = GetCell(positionSpawn);
            float minDistanceSqr = _minDistanceToSpawn * _minDistanceToSpawn;
            
            // Проверяем только соседние ячейки (3x3)
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    Vector2Int cell = new Vector2Int(centerCell.x + x, centerCell.y + y);
                    if (!_grid.TryGetValue(cell, out HashSet<GrassController> grasses)) continue;
                    foreach (var grass in grasses)
                    {
                        if (grass != null && 
                            (grass.transform.position - positionSpawn).sqrMagnitude < minDistanceSqr)
                        {
                            return false;
                        }
                    }
                }
            }
            
            return true;
        }
        
        private void AddToGrid(GrassController grass)
        {
            Vector2Int cell = GetCell(grass.transform.position);
            if (!_grid.ContainsKey(cell))
                _grid[cell] = new HashSet<GrassController>();
            
            _grid[cell].Add(grass);
            grass.CurrentCell = cell;
        }
        
        private void RemoveFromGrid(GrassController grass)
        {
            Vector2Int cell = grass.CurrentCell;
            if (_grid.TryGetValue(cell, out HashSet<GrassController> grasses))
            {
                grasses.Remove(grass);
                if (grasses.Count == 0)
                    _grid.Remove(cell);
            }
        }
        
        private Vector2Int GetCell(Vector3 position)
        {
            return new Vector2Int(
                Mathf.FloorToInt(position.x / _cellSize),
                Mathf.FloorToInt(position.z / _cellSize)
            );
        }
        
        public void ClearAllGrass()
        {
            foreach (var grass in _controllers)
            {
                if (grass != null && grass.gameObject != null)
                    Destroy(grass.gameObject);
            }
            
            _controllers.Clear();
            _grid.Clear();
        }

        public async UniTask<GrassController> FindCloserGrass(Vector3 position, CancellationToken token = default)
        {
            return _controllers.Count switch
            {
                <= 0 => null,
                < 31 => await FindCloserGrassBruteForce(position, token),
                _ => await FindCloserGrassWithGrid(position, token)
            };
        }
        
        private async UniTask<GrassController> FindCloserGrassBruteForce(Vector3 position, CancellationToken token)
        {
            GrassController closest = null;
            float minSqrDistance = float.MaxValue;
        
            // Разбиваем на чанки для сохранения отзывчивости
            const int CHUNK_SIZE = 10;
        
            for (int i = 0; i < _controllers.Count; i += CHUNK_SIZE)
            {
                int end = Mathf.Min(i + CHUNK_SIZE, _controllers.Count);
            
                for (int j = i; j < end; j++)
                {
                    var grass = _controllers[j];
                    if (!grass || !grass.IsEmpty) continue;
                
                    float sqrDist = (grass.transform.position - position).sqrMagnitude;
                    if (sqrDist < minSqrDistance)
                    {
                        minSqrDistance = sqrDist;
                        closest = grass;
                    }
                }
            
                // Yield каждые CHUNK_SIZE объектов
                if (i + CHUNK_SIZE < _controllers.Count)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
            }
            
            return closest;
        }
        
        private async UniTask<GrassController> FindCloserGrassWithGrid(Vector3 position, CancellationToken token = default)
        {
            if (_grid.Count == 0)
                return null;

            Vector2Int centerCell = GetCell(position);
            GrassController closest = null;
            float minSqrDistance = float.MaxValue;

            // Проверяем центральную ячейку
            if (_grid.TryGetValue(centerCell, out HashSet<GrassController> centerGrasses))
            {
                foreach (var grass in centerGrasses)
                {
                    if (grass == null) continue;
                    float sqrDist = (grass.transform.position - position).sqrMagnitude;
                    if (sqrDist < minSqrDistance)
                    {
                        minSqrDistance = sqrDist;
                        closest = grass;
                    }
                }

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            int ring = 1;
            // Если объект уже найден, ограничиваем радиус поиска
            float searchRadius = closest != null ? Mathf.Sqrt(minSqrDistance) : float.MaxValue;

            while (true)
            {
                // Если известен радиус поиска и текущее кольцо начинается за его пределами — выходим
                if (closest != null)
                {
                    float minDistanceToRing = (ring - 1) * _cellSize;
                    if (minDistanceToRing > searchRadius)
                        break;
                }

                bool anyCellProcessed = false;
                for (int dx = -ring; dx <= ring; dx++)
                {
                    for (int dy = -ring; dy <= ring; dy++)
                    {
                        // Пропускаем внутренние кольца (уже проверены)
                        if (Mathf.Abs(dx) < ring && Mathf.Abs(dy) < ring)
                            continue;

                        Vector2Int cell = new Vector2Int(centerCell.x + dx, centerCell.y + dy);
                        if (_grid.TryGetValue(cell, out HashSet<GrassController> grasses))
                        {
                            anyCellProcessed = true;
                            foreach (var grass in grasses)
                            {
                                if (grass == null) continue;
                                float sqrDist = (grass.transform.position - position).sqrMagnitude;
                                if (sqrDist < minSqrDistance)
                                {
                                    minSqrDistance = sqrDist;
                                    closest = grass;
                                    searchRadius = Mathf.Sqrt(minSqrDistance);
                                }
                            }
                        }
                        await UniTask.Yield(PlayerLoopTiming.Update, token);
                    }
                    await UniTask.Yield(PlayerLoopTiming.Update, token);
                }
                
                if (!anyCellProcessed && closest != null)
                    break;

                ring++;
                // Защита от бесконечного цикла
                if (ring > 1000) break;

                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }

            return closest;
        }
    }
}
