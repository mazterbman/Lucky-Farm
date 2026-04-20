using Unity.AI.Navigation;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Installers
{
    public class MobInstaller : MonoInstaller
    {
        [SerializeField] private NavMeshSurface _meshSurface = null;
    
        public override void InstallBindings()
        {
            Container.Bind<NavMeshSurface>().FromInstance(_meshSurface).AsSingle();
        }
    }
}