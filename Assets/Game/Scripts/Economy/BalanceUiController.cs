using TMPro;
using UnityEngine;

namespace Game.Scripts.Economy
{
    public class BalanceUiController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private TMP_Text _text;

        public void UpdateMoney(int money)
        {
            _text.SetText(money.ToString("D"));
        }
    }
}
