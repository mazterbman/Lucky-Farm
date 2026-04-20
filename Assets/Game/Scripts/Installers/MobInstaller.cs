using Game.Scripts.Mobs;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Installers
{
    public class MobInstaller : MonoInstaller
    {
        [SerializeField] private MobData _mobData = null;
        public override void InstallBindings()
        {
            Container.Bind<MobData>().FromInstance(_mobData).AsSingle();
        }
    }
}