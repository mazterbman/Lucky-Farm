using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Interfaces;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Building
{
    public abstract class BuildingController : MonoBehaviour, IClickObj
    {
        [Header("References")] 
        [SerializeField] private ColliderListener _colliderListener;
        [SerializeField] private GameObject _lightningHolder;

        private PlayerMousePrefabController _mousePrefabController; 
        private CancellationTokenSource _tokenSource;
        private int _currentLevel = 1;

        private void Awake()
        {
            LoadSettings(_currentLevel);
            
            _colliderListener.OnTriggerEnterAction += TriggerEnter;
            _colliderListener.OnTriggerExitAction += TriggerExit;
            
            _lightningHolder.SetActive(false);
            
            DestroyTokenSource?.Dispose();
            DestroyTokenSource = new CancellationTokenSource();
            
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();
        }

        private void OnDestroy()
        {
            _colliderListener.OnTriggerEnterAction -= TriggerEnter;
            _colliderListener.OnTriggerExitAction -= TriggerExit;
            
            RemoveListeners();
            
            DestroyTokenSource?.Cancel();
            DestroyTokenSource?.Dispose();
            
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }

        public abstract void OnClick();
        protected abstract void RemoveListeners();
        protected abstract void LoadSettings(int level);
        protected CancellationTokenSource DestroyTokenSource;

        private void TriggerEnter(Collider other)
        {
            if (!other.CompareTag(StaticValues.MouseTag)) return;

            _mousePrefabController ??= other.GetComponent<PlayerMousePrefabController>();
            
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();

            TriggerTask(_tokenSource.Token, true).Forget();
        }

        private void TriggerExit(Collider other)
        {
            if (!other.CompareTag(StaticValues.MouseTag)) return;

            _mousePrefabController ??= other.GetComponent<PlayerMousePrefabController>();
            
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();
            
            TriggerTask(_tokenSource.Token, false).Forget();
        }

        private async UniTask TriggerTask(CancellationToken token, bool enter)
        {
            await UniTask.Yield(token);
            switch (enter)
            {
                case true:
                    _mousePrefabController.EnterBuilding(this);
                    _lightningHolder.SetActive(true);
                    break;
                
                case false:
                    _mousePrefabController.DisableObject();
                    _lightningHolder.SetActive(false);
                    break;
            }
        }
        
    }
}
