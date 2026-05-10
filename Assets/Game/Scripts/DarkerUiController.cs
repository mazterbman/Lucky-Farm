using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts
{
    public class DarkerUiController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private List<Image> _images;
        [SerializeField] private List<RawImage> _rawImages;
        [SerializeField] private List<TMP_Text> _texts;

        [Header("Settings")] 
        [SerializeField] [Range(0, 3)] private float _timeToChange;
        [SerializeField] private Color _colorDarker;

        private float _pastTime = 1;

        private CancellationTokenSource _tokenSource;
        private List<Color> _normalColorImages = new();
        private List<Color> _normalColorRawImages = new();
        private List<Color> _normalColorTexts = new();
        
        private void Awake()
        {
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();

            GetNormalColorsAsync(_tokenSource.Token).Forget();
        }

        private void OnDestroy()
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
        }

        
        public void ChangeColor(bool reverse)
        {
            _tokenSource?.Cancel();
            _tokenSource?.Dispose();
            _tokenSource = new CancellationTokenSource();

            ChangeColorAsync(reverse, _tokenSource.Token).Forget();
        }
        
        public float CurrentTime => Mathf.Clamp01(_pastTime / _timeToChange);

        
        private async UniTask ChangeColorAsync(bool reverse, CancellationToken token)
        {
            if (_timeToChange <= 0.0f)
            {
                ChangeColorImages(reverse ? 0 : 1);
                return;
            }
            
            var multi = reverse ? -1 : 1;
            while (!token.IsCancellationRequested)
            {
                var timeDelta = Time.deltaTime;
                _pastTime =Mathf.Clamp(_pastTime + timeDelta * multi, 0, _timeToChange);
                ChangeColorImages(CurrentTime);
                switch (CurrentTime)
                {
                    case >= 1 when !reverse:
                    case <= 0 when reverse:
                        return;
                    default:
                        await UniTask.Yield(token);
                        break;
                }
            }
        }
        
        private async UniTask GetNormalColorsAsync(CancellationToken token)
        {
            foreach (var image in _images)
            {
                if (image.gameObject && image)
                    _normalColorImages.Add(image.color);

                await UniTask.Yield(token);
            }
            
            foreach (var image in _rawImages)
            {
                if (image.gameObject && image)
                    _normalColorRawImages.Add(image.color);

                await UniTask.Yield(token);
            }
            
            foreach (var text in _texts)
            {
                if (text.gameObject && text)
                    _normalColorTexts.Add(text.color);

                await UniTask.Yield(token);
            }
        }

        private void ChangeColorImages(float time)
        {
            Color colorEnd;
            for (int i = 0; i < _normalColorImages.Count; i++)
            {
                colorEnd = _normalColorImages[i] * _colorDarker;
                _images[i].color = Color.Lerp(_normalColorImages[i], colorEnd, time);
            }
            
            for (int i = 0; i < _normalColorRawImages.Count; i++)
            {
                colorEnd = _normalColorRawImages[i] * _colorDarker;
                _rawImages[i].color = Color.Lerp(_normalColorRawImages[i], colorEnd, time);
            }
            
            for (int i = 0; i < _normalColorTexts.Count; i++)
            { 
                colorEnd = _normalColorTexts[i] * _colorDarker;
                _texts[i].color = Color.Lerp(_normalColorTexts[i], colorEnd, time);
            }
        }
    }
}