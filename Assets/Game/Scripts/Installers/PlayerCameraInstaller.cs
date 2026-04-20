using Game.Scripts.Player;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Installers
{
    public class PlayerCameraInstaller : MonoInstaller
    {
        [SerializeField] private PlayerCameraData _cameraData = null;
        
        public override void InstallBindings()
        {
            Container.Bind<PlayerCameraData>().FromInstance(_cameraData).AsSingle();
        }
    }
}