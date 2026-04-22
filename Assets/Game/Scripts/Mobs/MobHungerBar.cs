using UnityEngine;
using UnityEngine.UI;

namespace Game.Scripts.Mobs
{
    public class MobHungerBar:MonoBehaviour
    {
        [SerializeField] private Image _imageBar;
        
        public void ResetBar()
        {
            _imageBar.fillAmount = 1;
        }

        public void UpdateBar(float percent)
        {
            _imageBar.fillAmount = Mathf.Clamp01(percent);;
        }
    }
}