using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Scripts.Installers
{
    public class PlayerCameraInstaller : MonoInstaller
    {
        [SerializeField] private CinemachineSplineDolly _splineDolly = null;
        [SerializeField] private InputActionReference _scrollAction = null;
        [SerializeField] private Camera _camera;
        
        public override void InstallBindings()
        {
            Container.Bind<CinemachineSplineDolly>().FromInstance(_splineDolly).AsSingle();
            Container.Bind<InputActionReference>().FromInstance(_scrollAction).AsSingle();
            Container.Bind<Camera>().FromInstance(_camera).AsSingle();
        }
    }
}