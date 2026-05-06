using System.Threading;
using Cysharp.Threading.Tasks;
using Game.Scripts.Grass;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Mobs.Mob
{
    public class MobController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private MobPathController _pathController;
        [SerializeField] private MobHungerSystem _hungerSystem;
        [SerializeField] private MobAnimatorController _animatorController;

        [Inject] private GrassData _grassData;
        
        private GrassController _currentGrassController;
        private CancellationTokenSource _tokenSource;
        
        private void Start()
        {
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();
            
            _hungerSystem.OnStatusChange += MobStatusChange;
        }

        private void OnDestroy()
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            
            if (_currentGrassController) _currentGrassController.IsEmpty = true;
            _hungerSystem.OnStatusChange -= MobStatusChange;
        }

        private void MobStatusChange(MobHungerStatus arg0)
        {
            switch (arg0)
            {
                case MobHungerStatus.Normal:
                    break;
                
                case MobHungerStatus.Hungry:
                    FindFood().Forget();
                    break;
                
                case MobHungerStatus.VeryHungry:
                    Dead();
                    break;
            }
        }

        private void Dead()
        {
            //ToDo
            //Make Animation Dead
            
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            
            if (_currentGrassController) _currentGrassController.IsEmpty = true;
            Destroy(gameObject);
        }

        private async UniTask ReachedFood()
        {
            if (_currentGrassController)
            {
                await _animatorController.AnimateFood(_tokenSource.Token);
                if (_tokenSource.IsCancellationRequested)
                    return;

                _hungerSystem.EatFood();
                _grassData.Manager.RemoveGrass(_currentGrassController);
            }

            await UniTask.WaitForEndOfFrame(_tokenSource.Token);
            
            if (_hungerSystem.HungerStatus == MobHungerStatus.Hungry)
            {
                FindFood().Forget();
                return;
            }
            
            _pathController.ReturnToWandering();
        }

        private async UniTask FindFood()
        {
            _currentGrassController = null;
            while (!_currentGrassController && !_tokenSource.IsCancellationRequested)
            {
                await UniTask.WaitForEndOfFrame(_tokenSource.Token);
                _currentGrassController = await _grassData.Manager.FindCloserGrass(transform.position, _tokenSource.Token);
            }
            
            if (_tokenSource.Token.IsCancellationRequested || !_currentGrassController)
                return;

            _currentGrassController.IsEmpty = false;
            _currentGrassController.CurrentMobController = this;
            _pathController.MoveToTransform(_currentGrassController.transform, () => ReachedFood().Forget(), returnToWander: false);
        }
        
    }
}
