using UnityEngine;

namespace Game.Scripts.Player
{
    public class PlayerMousePrefabController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private GameObject _holderObj;
        
        private StateMousePrefab _state = StateMousePrefab.Disable;
        
        public void SetWorldPosition(Vector3 worldPosition)
        {
            transform.position = worldPosition;
        }

        public void EnterBuilding()
        {
            if (_state == StateMousePrefab.OnBuilding)
                return;

            _holderObj.SetActive(false);
            _state = StateMousePrefab.OnBuilding;
        }

        public void ExitBuilding()
        {
            if (_state != StateMousePrefab.OnBuilding)
                return;

            _holderObj.SetActive(false);
            _state = StateMousePrefab.Disable;
        }

        public void DisableObject()
        {
            if (_state == StateMousePrefab.Disable) 
                return;
            
            _holderObj.SetActive(false);
            _state = StateMousePrefab.Disable;
        }

        public void EnableObject()
        {
            if (_state != StateMousePrefab.Disable)
                return;
            
            _holderObj.SetActive(true);
            _state = StateMousePrefab.Enable;
        }
    }

    public enum StateMousePrefab
    {
        Disable = 0,
        Enable,
        OnBuilding,
    }
}
