using System;
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
        [Header("Debug Information")] 
        [SerializeField] private List<GrassController> _controllers;

        [Inject] private DiContainer _container;
        [Inject] private GrassData _grassData;

        private CancellationTokenSource _tokenSource;
        private Transform _grassParent;
        private AssetReferenceGameObject _grassPrefab;

        private void Awake()
        {
            _grassPrefab = _grassData.GrassPrefab;
            _grassParent = _grassData.Parent;
            
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
            }
            finally
            {
                Addressables.Release(operationHandle);
            }
        }
        
    }
}
