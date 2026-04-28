using Game.Scripts.Player;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Installers
{
    public class PlayerCameraInstaller : MonoInstaller
    {
        [SerializeField] private PlayerData _data = null;
        
        public override void InstallBindings()
        {
            Container.Bind<PlayerData>().FromInstance(_data).AsSingle();
        }
    }
}