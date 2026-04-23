using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Scripts.Grass
{
    public class GrassController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private AssetReference _materialReference;
        [SerializeField] private List<MeshRenderer> _meshRenderers;
        
        private CancellationTokenSource _tokenSource;
        
        private void Start()
        {
            transform.localScale = Vector3.one;
            IsEmpty = true;
            
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();
            SetMaterialsAsync().Forget();
        }

        private void OnDestroy()
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }

        private async UniTaskVoid SetMaterialsAsync()
        {
            if (_materialReference == null || _meshRenderers.Count <= 0)
                return;

            AsyncOperationHandle<Material> operationHandle = _materialReference.LoadAssetAsync<Material>();
            await operationHandle.Task.AsUniTask();
            if (_tokenSource.IsCancellationRequested)
                return;

            foreach (var meshRenderer in _meshRenderers)
            {
                meshRenderer.SetMaterials(new List<Material>());
                meshRenderer.sharedMaterial = operationHandle.Result;
            }
        }

        public bool IsEmpty { get; set; }
        public Vector2Int CurrentCell { get; set; }
        public void Remove() => Destroy(gameObject);
    }
}
