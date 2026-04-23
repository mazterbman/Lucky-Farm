using Game.Scripts.Building;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Installers
{
    public class BuildingInstaller : MonoInstaller
    {
        [SerializeField] private BuildingData _buildingData;

        public override void InstallBindings()
        {
            Container.Bind<BuildingData>().FromInstance(_buildingData).AsSingle();
        }
    }
}