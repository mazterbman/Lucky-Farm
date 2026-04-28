using Game.Scripts.Settings;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.Economy
{
    public class BalanceLevelManager : MonoBehaviour
    {
        public UnityAction OnUpdateBalance;

        [Inject] private SettingsLevelData _levelData;
        
        [Header("Debug Log")] 
        [SerializeField] [TextArea(3,10)] private string _debugString;
        
        private int _balance;

        private void Start()
        {
            _balance = _levelData.StartBalanceLevel;
            OnUpdateBalance += UpdateBalance;
        }

        private void OnDestroy()
        {
            OnUpdateBalance -= UpdateBalance;
        }

        public int Balance => _balance;

        public bool TryAdd(int money)
        {
            if (money <= 0)
            {
                _debugString = $"U try add InCorrect Money = {money}";
                return false;
            }

            _balance += money;
            OnUpdateBalance?.Invoke();
            return true;
        }

        public bool TryRemove(int money)
        {
            if (money <= 0)
            {
                _debugString = $"U try remove InCorrect Money = {money}";
                return false;
            }

            if (_balance < money)
            {
                _debugString = $"U try remove TOO MANY Money = {money} from Balance = {_balance}";
                return false;
            }
            
            _balance -= money;
            OnUpdateBalance?.Invoke();
            return true;
        }

        private void UpdateBalance()
        {
            _debugString = $"CurrentBalance = {_balance}";
        }
        
    }
}