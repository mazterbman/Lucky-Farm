using Game.Scripts.Building;
using Game.Scripts.Economy;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Installers
{
    public class EconomyInstaller: MonoInstaller
    {
        [SerializeField] private EconomyData _economyData;

        public override void InstallBindings()
        {
            Container.Bind<EconomyData>().FromInstance(_economyData).AsSingle();
        }
    }
}