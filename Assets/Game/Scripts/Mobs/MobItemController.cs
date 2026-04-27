using System;
using Game.Scripts.Building;
using Game.Scripts.Building.StoreHouse;
using Game.Scripts.Interfaces;
using Game.Scripts.Player;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Mobs
{
    public class MobItemController : MonoBehaviour, IClickObj
    {
        [Header("References")]
        [SerializeField] private ColliderListener _colliderListener;
        [SerializeField] private GameObject _lightingHolder;
        
        [Header("Settings")] 
        [SerializeField] private StoreItem.TypeItem _itemType = StoreItem.TypeItem.Egg;

        private StoreItem _item;
        [Inject] private BuildingData _buildingData;

        private void Awake()
        {
            _colliderListener.OnTriggerEnterAction += TriggerEnter;
            _colliderListener.OnTriggerExitAction += TriggerExit;
            
            _lightingHolder.SetActive(false);

            switch (_itemType)
            {
                case StoreItem.TypeItem.Egg:
                    _item = new StoreItem().CreateEgg();
                    break;
            }
        }

        private void OnDestroy()
        {
            _colliderListener.OnTriggerEnterAction -= TriggerEnter;
            _colliderListener.OnTriggerExitAction -= TriggerExit;
        }

        private void TriggerExit(Collider other)
        {
            if (!other.CompareTag(StaticValues.MouseTag))
                return;

            PlayerMousePrefabController controller = other.GetComponent<PlayerMousePrefabController>();
            controller.DisableObject();
            _lightingHolder.SetActive(false);
        }

        private void TriggerEnter(Collider other)
        {
            if (!other.CompareTag(StaticValues.MouseTag))
                return;
            
            PlayerMousePrefabController controller = other.GetComponent<PlayerMousePrefabController>();
            controller.EnterItem(this);
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