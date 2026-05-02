using Game.Scripts.Items;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Installers
{
    [CreateAssetMenu(fileName = "ItemsInstaller", menuName = "Installers/ItemsInstaller")]
    public class ItemsInstaller : ScriptableObjectInstaller<ItemsInstaller>
    {
        [SerializeField] private ItemsStorage _itemsStorage;
        public override void InstallBindings()
        {
            Container.Bind<ItemsStorage>().FromInstance(_itemsStorage).AsSingle();
        }
    }
}