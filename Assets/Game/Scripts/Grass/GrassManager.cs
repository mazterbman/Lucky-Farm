using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Zenject;

namespace Game.Scripts.Grass
{
    public class GrassManager : MonoBehaviour
    {
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

        private void Awake()
        {
            _grassPrefab = _grassData.GrassPrefab;
            _grassParent = _grassData.Parent;
            _grid = new Dictionary<Vector2Int, HashSet<GrassController>>();
            
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
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
                
                // // Подписываемся на событие удаления (если есть)
                // controller.OnDestroyed += () => RemoveGrass(controller);
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
            
            // // Уничтожаем объект
            // if (controller.gameObject != null)
            //     Destroy(controller.gameObject);
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
        
    }
}
