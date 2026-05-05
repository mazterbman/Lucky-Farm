using System;
using System.Collections.Generic;
using Game.Scripts.Building;
using Game.Scripts.Grass;
using Game.Scripts.Mobs;
using Game.Scripts.Mobs.Mob;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Player
{
    public class PlayerMousePrefabController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private GameObject _holderObj;
        
        [Inject] private GrassData _grassData;
        
        private BuildingController _buildingController;
        private List<MobItemController> _mobItemControllers = new List<MobItemController>();
        private StateMousePrefab _state = StateMousePrefab.Disable;

        public void SetWorldPosition(Vector3 worldPosition)
        {
            transform.position = worldPosition;
        }

        public void EnterBuilding(BuildingController controller)
        {
            if (_state is StateMousePrefab.OnBuilding or StateMousePrefab.OnItem)
                return;

            _buildingController = controller;
            _holderObj.SetActive(false);
            _state = StateMousePrefab.OnBuilding;
        }

        public void EnterItem(MobItemController controller)
        {
            if (_state is StateMousePrefab.OnBuilding)
                return;

            _mobItemControllers.Add(controller);
            _holderObj.SetActive(false);
            _state = StateMousePrefab.OnItem;
        }

        public void ExitItem(MobItemController controller)
        {
            _mobItemControllers.Remove(controller);
            if (_mobItemControllers.Count <= 0 && !_buildingController)
            {
                DisableObject();
            }
        }
        
        public void OnGrass()
        {
            if (_state != StateMousePrefab.Disable)
                return;
            
            _holderObj.SetActive(true);
            _state = StateMousePrefab.OnGrassSpace;
        }

        public void DisableObject()
        {
            if (_state == StateMousePrefab.Disable) 
                return;
            
            _holderObj.SetActive(false);
            _buildingController = null;
            transform.position = Vector3.one * -10;
            _state = StateMousePrefab.Disable;
        }

        public void ClkOnObject()
        {
            switch (_state)
            {
                case StateMousePrefab.OnGrassSpace:
                    _grassData.Manager.CreateGrass(transform.position).Forget();
                    break;
                
                case StateMousePrefab.OnBuilding:
                    _buildingController.OnClick();
                    break;
                
                case StateMousePrefab.OnAnimal:
                    break;
                
                case StateMousePrefab.OnItem:
                    _mobItemControllers.ForEach(item => item.OnClick());
                    DisableObject();
                    break;
                
                default: break;
            }
        }
    }

    public enum StateMousePrefab
    {
        Disable = 0,
        OnGrassSpace,
        OnBuilding,
        OnAnimal,
        OnItem
    }
}
