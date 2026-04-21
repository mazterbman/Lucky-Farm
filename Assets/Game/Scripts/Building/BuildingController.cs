using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Building
{
    public class BuildingController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private BuildingColliderListener _colliderListener;
        [SerializeField] private GameObject _lightningHolder;

        private CancellationTokenSource _tokenSource;
        private static readonly string TagMouse = "PlayerMouse";

        private void Awake()
        {
            _colliderListener.OnTriggerEnterAction += TriggerEnter;
            _colliderListener.OnTriggerExitAction += TriggerExit;
            
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();
        }

        private void Start()
        {
            _lightningHolder.SetActive(false);
        }

        private void OnDestroy()
        {
            _colliderListener.OnTriggerEnterAction -= TriggerEnter;
            _colliderListener.OnTriggerExitAction -= TriggerExit;
            
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }

        public void OnClick()
        {
            
        }

        private void TriggerEnter(Collider other)
        {
            if (!other.CompareTag(TagMouse)) return;

            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();

            EnterTask(other, _tokenSource.Token).Forget();
        }

        private void TriggerExit(Collider other)
        {
            if (!other.CompareTag(TagMouse)) return;

            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();

            ExitTask(other, _tokenSource.Token).Forget();
        }

        private async UniTask ExitTask(Collider other, CancellationToken token)
        {
            await UniTask.Yield(token);
            
            PlayerMousePrefabController controller = other.GetComponent<PlayerMousePrefabController>();
            controller.ExitBuilding();
            _lightningHolder.SetActive(false);
        }
        
        private async UniTask EnterTask(Collider other, CancellationToken token)
        {
            await UniTask.Yield(token);
            
            PlayerMousePrefabController controller = other.GetComponent<PlayerMousePrefabController>();
            controller.EnterBuilding(this);
            _lightningHolder.SetActive(true);
        }
        
    }
}
