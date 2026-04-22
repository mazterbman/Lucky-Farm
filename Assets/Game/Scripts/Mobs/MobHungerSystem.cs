using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Game.Scripts.Mobs
{
    public class MobHungerSystem : MonoBehaviour
    {
        public UnityAction<MobHungerStatus> OnStatusChange;
        
        [Header("References")] 
        [SerializeField] private MobHungerBar _hungerBar;

        [Header("Settings")] 
        [SerializeField] private float _maxHunger;
        [SerializeField] private float _damagePerSec;

        [Space] 
        [SerializeField] [Range(0, 1)] private float _percentHunger;
        [SerializeField] [Range(0, 0.2f)] private float _randomHungerFactor;

        [Header("Debug Log")] 
        [SerializeField] [TextArea(5, 10)] private string _debugString;

        private MobHungerStatus _hungerStatus;
        private float _currentHunger = 0;

        private void Start()
        {
            _hungerBar.ResetBar();
            _hungerStatus = MobHungerStatus.Normal;
            _currentHunger = _maxHunger;

            float randomFactor = Random.Range(0, _randomHungerFactor);
            _percentHunger += Random.Range(0, 2) == 0 ? -randomFactor : randomFactor;
        }

        private void Update()
        {
            Damage();
        }

        private void Damage()
        {
            _currentHunger = Mathf.Clamp(_currentHunger -_damagePerSec * Time.deltaTime, 0, _maxHunger);
            _hungerBar.UpdateBar(PercentCurrentHunger);

            _debugString = $"Current Health = {_currentHunger}";
            _debugString += $"\nPercent Health = {PercentCurrentHunger}";
            
            if (PercentCurrentHunger <= 0)
            {
                ChangeStatus(MobHungerStatus.Dead);
                return;
            }
            
            if (PercentCurrentHunger < _percentHunger)
            {
                ChangeStatus(MobHungerStatus.Hungry);
                return;
            }
            
            ChangeStatus(MobHungerStatus.Normal);
        }

        private void ChangeStatus(MobHungerStatus newStatus)
        {
            if (newStatus == _hungerStatus)
                return;

            _hungerStatus = newStatus;
            OnStatusChange?.Invoke(newStatus);
        }


        public MobHungerStatus HungerStatus => _hungerStatus;
        public void EatFood() => _currentHunger += _maxHunger / 2;
        public float CurrentHunger => _currentHunger;
        public float PercentCurrentHunger => _currentHunger / _maxHunger;
    }

    public enum MobHungerStatus
    {
        Normal = 0,
        Hungry,
        Dead,
    }
}
