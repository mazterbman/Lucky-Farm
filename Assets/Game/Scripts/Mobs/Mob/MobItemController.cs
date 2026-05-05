using Game.Scripts.Building;
using Game.Scripts.Building.StoreHouse;
using Game.Scripts.Interfaces;
using Game.Scripts.Items;
using Game.Scripts.Player;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Mobs.Mob
{
    public class MobItemController : MonoBehaviour, IClickObj
    {
        [Header("References")]
        [SerializeField] private ColliderListener _colliderListener;
        [SerializeField] private GameObject _lightingHolder;
        
        [Header("Settings")] 
        [SerializeField] private Items.TypeItem _itemType = Items.TypeItem.Egg;

        private StoreItem _item;
        private PlayerMousePrefabController _mousePrefabController;
        [Inject] private ItemsStorage _itemsStorage;
        [Inject] private BuildingData _buildingData;

        private void Awake()
        {
            _colliderListener.OnTriggerEnterAction += TriggerEnter;
            _colliderListener.OnTriggerExitAction += TriggerExit;
            
            _lightingHolder.SetActive(false);
            if (_itemsStorage.TryGetItem(_itemType, out var item))
            {
                _item = new StoreItem(item, 1);
            }
        }

        private void OnDestroy()
        {
            _colliderListener.OnTriggerEnterAction -= TriggerEnter;
            _colliderListener.OnTriggerExitAction -= TriggerExit;

            if (_mousePrefabController)
            {
                _mousePrefabController.ExitItem(this);
            }
        }

        private void TriggerExit(Collider other)
        {
            if (!other.CompareTag(StaticValues.MouseTag))
                return;

            _mousePrefabController ??= other.GetComponent<PlayerMousePrefabController>();
            _mousePrefabController.ExitItem(this);
            _lightingHolder.SetActive(false);
        }

        private void TriggerEnter(Collider other)
        {
            if (!other.CompareTag(StaticValues.MouseTag))
                return;
            
            _mousePrefabController ??= other.GetComponent<PlayerMousePrefabController>();
            _mousePrefabController.EnterItem(this);
            _lightingHolder.SetActive(true);
        }

        public void OnClick()
        {
            _buildingData.StoreHouseController.TryAddItem(_item);
            
            //TODO
            // Made effect or animate for collect Item
            Destroy(gameObject);
        }
    }
}