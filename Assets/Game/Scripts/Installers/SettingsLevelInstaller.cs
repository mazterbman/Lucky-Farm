using Game.Scripts.Settings;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Installers
{
    [CreateAssetMenu(fileName = "SettingsLevelInstaller", menuName = "Installers/SettingsLevelInstaller")]
    public class SettingsLevelInstaller : ScriptableObjectInstaller<SettingsLevelInstaller>
    {
        [SerializeField] private SettingsLevelData _data = null;
        
        public override void InstallBindings()
        {
            Container.Bind<SettingsLevelData>().FromInstance(_data).AsSingle();
        }
    }
}