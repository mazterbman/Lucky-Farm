using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Economy
{
    public class BalanceLevelManager : MonoBehaviour
    {
        [Header("Settings")] 
        [SerializeField] [Range(0, 1000)] private int _statBalance = 0;
        
        [Header("Debug Log")] 
        [SerializeField] [TextArea(3,10)] private string _debugString;
        
        private float _balance;
        private UnityAction OnUpdateBalance;

        private void Start()
        {
            _balance = _statBalance;
            OnUpdateBalance += UpdateBalance;
        }

        private void OnDestroy()
        {
            OnUpdateBalance -= UpdateBalance;
        }

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