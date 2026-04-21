using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts.Building
{
    public class BuildingColliderListener : MonoBehaviour
    {
        public UnityAction<Collider> OnTriggerEnterAction;
        public UnityAction<Collider> OnTriggerExitAction; 
        
        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterAction.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitAction.Invoke(other);
        }
    }
}
