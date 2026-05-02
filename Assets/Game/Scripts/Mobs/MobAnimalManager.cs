using Game.Scripts.Building.StoreHouse;
using Game.Scripts.Items;
using Game.Scripts.Settings;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Mobs
{
    public class MobAnimalManager : MonoBehaviour
    {
        [Inject] private MobData _mobData;
        [Inject] private SettingsLevelData _levelData;

        private void Start()
        {
            SpawnAnimals();
        }

        public bool TrySellAnimal(StoreItem item)
        {
            if (item == null)
            {
                return false;
            }
            
            switch (item.Item.Type)
            {
                case TypeItem.Chicken:
                    for (int i = 0; i < item.Count; i++)
                    {
                        _mobData.MobAnimalSpawner.RemoveChicken();
                    }
                    return true;

                case TypeItem.Egg:
                default:
                    return false;
            }
        }

        private void SpawnAnimals()
        {
            for (int i = 0; i < _levelData.StartChickenCount; i++)
            {
                _mobData.MobAnimalSpawner.SpawnChicken(true).Forget();
            }
        }
    }
}
