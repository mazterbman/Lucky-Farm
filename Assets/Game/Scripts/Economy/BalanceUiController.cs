using System;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Economy
{
    public class BalanceUiController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private TMP_Text _text;

        [Inject] private EconomyData _economyData;

        private void Awake()
        {
            _economyData.BalanceLevelManager.OnUpdateBalance += UpdateMoney;
        }

        private void Start()
        {
            UpdateMoney();
        }

        private void OnDestroy()
        {
            _economyData.BalanceLevelManager.OnUpdateBalance -= UpdateMoney;
        }

        private void UpdateMoney()
        {
            _text.SetText(_economyData.BalanceLevelManager.Balance.ToString("D"));
        }
    }
}
