using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Scripts.Mobs
{
    public class MobItemController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private GameObject _itemForSpawn;

        [Header("Settings")]
        [SerializeField] [Range(0, 600)] private float _timeForSpawn = 500;
        [SerializeField] [Range(0, 30)] private float _randomizeTimeForSpawn = 15;

        private CancellationTokenSource _tokenSource;
        private float _currentTime;

        private void Start()
        {
            _currentTime = ApplyRandomizeTime();
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();
            
            SpawnAsync(_tokenSource.Token).Forget();
        }

        private void OnDestroy()
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }

        private async UniTask SpawnAsync(CancellationToken token)
        {
            float time = 0;
            while (time < _currentTime && token.IsCancellationRequested)
            {
                time += Time.deltaTime;
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
            
            if(token.IsCancellationRequested)
                return;
            
            SpawnItem();
        }

        private float ApplyRandomizeTime()
        {
            var rand = Random.Range(0,2) == 0 ? Random.Range(0, _randomizeTimeForSpawn) : Random.Range(-_randomizeTimeForSpawn, 0);
            return rand + _timeForSpawn;
        }

        private void SpawnItem()
        {
            
        }
        
    }
}
