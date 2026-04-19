using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Scripts.Installers
{
    public class PlayerCameraInstaller : MonoInstaller
    {
        [SerializeField]
        private CinemachineSplineDolly _splineDolly = null;
        
        [SerializeField]
        private InputActionReference _scrollAction = null;
        
        public override void InstallBindings()
        {
            Container.Bind<CinemachineSplineDolly>().FromInstance(_splineDolly).AsSingle();
            Container.Bind<InputActionReference>().FromInstance(_scrollAction).AsSingle();
        }
    }
}