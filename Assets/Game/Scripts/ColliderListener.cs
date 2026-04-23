using System;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Scripts
{
    public class ColliderListener : MonoBehaviour
    {
        public UnityAction<Collider> OnTriggerEnterAction;
        public UnityAction<Collider> OnTriggerExitAction;
        public UnityAction<Collider> OnTriggerStayAction;
        
        private void OnTriggerEnter(Collider other)
        {
            OnTriggerEnterAction?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            OnTriggerExitAction?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            OnTriggerStayAction?.Invoke(other);
        }
    }
}
