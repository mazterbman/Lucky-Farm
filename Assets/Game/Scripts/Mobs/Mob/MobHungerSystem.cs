using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Game.Scripts.Mobs.Mob
{
    public class MobHungerSystem : MonoBehaviour
    {
        public UnityAction<MobHungerStatus> OnStatusChange;
        
        [Header("References")] 
        [SerializeField] private ResourceBar _hungerBar;

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
        private float _currentPercentFactor;

        private void Start()
        {
            _hungerBar.ResetBar();
            _hungerStatus = MobHungerStatus.Normal;
            _currentHunger = _maxHunger;

            RandomiseHunger();
        }

        private void Update()
        {
            Damage();
        }

        private void Damage()
        {
            _currentHunger = Mathf.Clamp(_currentHunger -_damagePerSec * Time.deltaTime, 0, _maxHunger);
            _hungerBar.UpdateBar(PercentCurrentHunger);

            _debugString = $"Current Hunger = {_currentHunger}";
            _debugString += $"\nPercent Hunger = {PercentCurrentHunger}";
            
            if (PercentCurrentHunger <= 0)
            {
                ChangeStatus(MobHungerStatus.Dead);
                return;
            }
            
            if (PercentCurrentHunger < _currentPercentFactor)
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

            if (newStatus == MobHungerStatus.Normal)
            {
                RandomiseHunger();
            }
            
            _hungerStatus = newStatus;
            OnStatusChange?.Invoke(newStatus);
        }
        
        private void RandomiseHunger()
        {
            float randomFactor = Random.Range(0, _randomHungerFactor);
            _currentPercentFactor = _percentHunger + (Random.Range(0, 2) == 0 ? -randomFactor : randomFactor);
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
