using Game.Scripts.Grass;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Installers
{
    public class GrassInstaller : MonoInstaller
    {
        [SerializeField] private GrassData _grassData;

        public override void InstallBindings()
        {
            Container.Bind<GrassData>().FromInstance(_grassData).AsSingle();
        }
    }
}