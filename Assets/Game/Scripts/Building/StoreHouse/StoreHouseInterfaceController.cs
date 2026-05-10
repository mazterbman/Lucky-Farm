using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Building.StoreHouse
{
    public class StoreHouseInterfaceController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Canvas _canvasInterface;
        
        [Space]
        [SerializeField] private RectTransform _trackImage;
        [SerializeField] private TMP_Text _timeText;

        [Header("Settings for track")] 
        [SerializeField] private Vector2 _startPosition;
        [SerializeField] private Vector2 _endPosition;

        [Header("Settings")] 
        [SerializeField] private int _secondsOnSale;

        [Inject] private BuildingData _buildingData;

        private void Awake()
        {
            _canvasInterface.gameObject.SetActive(true);
            _timeText.gameObject.SetActive(false);
        }

        public void SetSecondsOnSale(int seconds) => _secondsOnSale = seconds;
        public void EnableInterface(bool enable) => _canvasInterface?.gameObject.SetActive(enable);
        
        public async UniTask MoveToSellItemsAsync(int countOfMoney, CancellationToken token)
        {
            TimerUpdateAsync(_secondsOnSale, token).Forget();
            await MoveTrackAsync(_secondsOnSale, token);
            _buildingData.StoreHouseController.AddOnBalance(countOfMoney);
        }

        private async UniTask MoveTrackAsync(int seconds, CancellationToken token)
        {
            float timePast = 0;
            float timeEnd = seconds / 2.0f;
            while (timePast < timeEnd && !token.IsCancellationRequested)
            {
                await UniTask.Yield(token);
                timePast += Time.deltaTime;
                _trackImage.anchoredPosition = Vector2.Lerp(_startPosition, _endPosition, timePast / timeEnd);
            }
            
            if (token.IsCancellationRequested)
                return;
            _trackImage.anchoredPosition = _endPosition;

            timePast = 0;
            while (timePast < timeEnd && !token.IsCancellationRequested)
            {
                await UniTask.Yield(token);
                timePast += Time.deltaTime;
                _trackImage.anchoredPosition = Vector2.Lerp(_endPosition, _startPosition, timePast / timeEnd);
            }
            
            if (token.IsCancellationRequested)
                return;
            
            _trackImage.anchoredPosition = _startPosition;
        }

        private async UniTask TimerUpdateAsync(float time, CancellationToken token)
        {
            _timeText.gameObject.SetActive(true);
            while (time > 0 && !token.IsCancellationRequested)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);
                time -= Time.deltaTime;
                UpdateTextTime(time);
            }
        }

        private void UpdateTextTime(float time)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            _timeText.SetText(timeSpan.ToString(@"mm\:ss"));
        }
    }
}
