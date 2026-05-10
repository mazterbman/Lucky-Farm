using System;
using Game.Scripts.Settings;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Game.Scripts.Economy
{
    public class BalanceLevelManager : MonoBehaviour
    {
        public UnityAction OnUpdateBalance;

        [SerializeField] private Canvas _interfaceBalance;
        
        [Header("Debug Log")] 
        [SerializeField] [TextArea(3,10)] private string _debugString;
        
        [Inject] private SettingsLevelData _levelData;
        
        private int _balance;

        private void Awake()
        {
            _balance = _levelData.StartBalanceLevel;
            OnUpdateBalance += UpdateBalance;
            UpdateBalance();
        }

        private void OnDestroy()
        {
            OnUpdateBalance -= UpdateBalance;
        }

        public int Balance => _balance;
        public void EnableInterface(bool enable) => _interfaceBalance?.gameObject.SetActive(enable);
        
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