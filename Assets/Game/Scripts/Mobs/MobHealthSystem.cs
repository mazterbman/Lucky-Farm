using UnityEngine;

namespace Game.Scripts.Mobs
{
    public class MobHealthSystem : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private MobHeathBar _heathBar;

        [Header("Settings")] 
        [SerializeField] private float _maxHeath;
        [SerializeField] private float _damagePerSec;

        [Space] 
        [SerializeField] [Range(0, 1)] private float _percentHunger;
        [SerializeField] [Range(0, 0.2f)] private float _randomHungerFactor;

        [Header("Debug Log")] 
        [SerializeField] [TextArea(5, 10)] private string _debugString;

        private MobHealthStatus _healthStatus;
        private float _currentHealth = 0;

        private void Start()
        {
            _heathBar.ResetBar();
            _healthStatus = MobHealthStatus.Normal;
            _currentHealth = _maxHeath;

            float randomFactor = Random.Range(0, _randomHungerFactor);
            _percentHunger += Random.Range(0, 2) == 0 ? -randomFactor : randomFactor;
        }

        private void Update()
        {
            Damage();
        }

        private void Damage()
        {
            _currentHealth = Mathf.Clamp(_currentHealth -_damagePerSec * Time.deltaTime, 0, _maxHeath);
            _heathBar.UpdateBar(PercentCurrentHealth);

            _debugString = $"Current Health = {_currentHealth}";
            _debugString += $"\nPercent Health = {PercentCurrentHealth}";
            
            if (PercentCurrentHealth <= 0)
            {
                Debug.Log("DEAD!!!");
                _healthStatus = MobHealthStatus.Dead;
                return;
            }
            
            if (PercentCurrentHealth < _percentHunger)
            {
                Debug.Log("Need Food NOW!");
                _healthStatus = MobHealthStatus.Hungry;
                return;
            }

            _healthStatus = MobHealthStatus.Normal;
        }
        
        
        public float CurrentHealth => _currentHealth;
        public float PercentCurrentHealth => _currentHealth / _maxHeath;
    }

    public enum MobHealthStatus
    {
        Normal = 0,
        Hungry,
        Dead,
    }
}
