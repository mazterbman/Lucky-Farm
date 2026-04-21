using Game.Scripts.Building;
using Game.Scripts.Grass;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Player
{
    public class PlayerMousePrefabController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private GameObject _holderObj;

        [Inject] private GrassData _grassData;
        
        private GrassManager _grassManager;
        private BuildingController _currentBuildingController;
        private StateMousePrefab _state = StateMousePrefab.Disable;

        private void Awake()
        {
            _grassManager = _grassData.Manager;
        }

        public void SetWorldPosition(Vector3 worldPosition)
        {
            transform.position = worldPosition;
        }

        public void EnterBuilding(BuildingController controller)
        {
            if (_state == StateMousePrefab.OnBuilding)
                return;

            _currentBuildingController = controller;
            _holderObj.SetActive(false);
            _state = StateMousePrefab.OnBuilding;
        }

        public void ExitBuilding()
        {
            if (_state != StateMousePrefab.OnBuilding)
                return;

            _currentBuildingController = null;
            _holderObj.SetActive(false);
            _state = StateMousePrefab.Disable;
        }
        
        public void EnterGrass()
        {
            if (_state is StateMousePrefab.OnGrass or StateMousePrefab.OnBuilding )
                return;
            
            _holderObj.SetActive(true);
            _state = StateMousePrefab.OnGrass;
        }

        public void DisableObject()
        {
            if (_state == StateMousePrefab.Disable) 
                return;
            
            _holderObj.SetActive(false);
            _state = StateMousePrefab.Disable;
        }

        public void ClkOnObject()
        {
            switch (_state)
            {
                case StateMousePrefab.OnGrass:
                    _grassManager.CreateGrass(transform.position).Forget();
                    break;
                
                case StateMousePrefab.OnBuilding:
                    _currentBuildingController.OnClick();
                    break;
                
                case StateMousePrefab.OnAnimal:
                    break;
                
                default: break;
            }
        }
    }

    public enum StateMousePrefab
    {
        Disable = 0,
        OnGrass,
        OnBuilding,
        OnAnimal
    }
}
