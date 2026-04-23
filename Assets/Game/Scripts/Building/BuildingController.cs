using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Building
{
    public abstract class BuildingController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private ColliderListener _colliderListener;
        [SerializeField] private GameObject _lightningHolder;

        private CancellationTokenSource _tokenSource;

        private void Awake()
        {
            _colliderListener.OnTriggerEnterAction += TriggerEnter;
            _colliderListener.OnTriggerExitAction += TriggerExit;
            
            _lightningHolder.SetActive(false);
            
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _colliderListener.OnTriggerEnterAction -= TriggerEnter;
            _colliderListener.OnTriggerExitAction -= TriggerExit;
            
            RemoveListeners();
            
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }

        public abstract void OnClick();
        protected abstract void RemoveListeners();

        private void TriggerEnter(Collider other)
        {
            if (!other.CompareTag(StaticValues.MouseTag)) return;

            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();

            TriggerTask(other, _tokenSource.Token, true).Forget();
        }

        private void TriggerExit(Collider other)
        {
            if (!other.CompareTag(StaticValues.MouseTag)) return;

            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();

            TriggerTask(other, _tokenSource.Token, false).Forget();
        }

        private async UniTask TriggerTask(Collider other, CancellationToken token, bool enter)
        {
            await UniTask.Yield(token);
            PlayerMousePrefabController controller = other.GetComponent<PlayerMousePrefabController>();
            switch (enter)
            {
                case true:
                    controller.EnterBuilding(this);
                    _lightningHolder.SetActive(true);
                    break;
                
                case false:
                    controller.ExitBuilding();
                    _lightningHolder.SetActive(false);
                    break;
            }
        }
        
    }
}
