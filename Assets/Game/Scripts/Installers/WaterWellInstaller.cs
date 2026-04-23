using Game.Scripts.Building;
using UnityEngine;
using Zenject;

public class WaterWellInstaller : MonoInstaller
{
    [SerializeField] private WaterWellData _waterWellData;

    public override void InstallBindings()
    {
        Container.Bind<WaterWellData>().FromInstance(_waterWellData).AsSingle();
    }
}