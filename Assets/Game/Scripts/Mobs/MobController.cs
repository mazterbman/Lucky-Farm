using UnityEngine;

namespace Game.Scripts.Mobs
{
    public class MobController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private MobPathController _pathController;
        [SerializeField] private MobHealthSystem _healthSystem;
    }
}
