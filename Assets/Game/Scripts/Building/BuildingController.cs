using System;
using Game.Scripts.Player;
using UnityEngine;

namespace Game.Scripts.Building
{
    public class BuildingController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private BuildingColliderListener _colliderListener;
        [SerializeField] private GameObject _lightningHolder;
        
        private static readonly string TagMouse = "PlayerMouse";

        private void Awake()
        {
            _colliderListener.OnTriggerEnterAction += TriggerEnter;
            _colliderListener.OnTriggerExitAction += TriggerExit;
        }

        private void Start()
        {
            _lightningHolder.SetActive(false);
        }

        private void OnDestroy()
        {
            _colliderListener.OnTriggerEnterAction -= TriggerEnter;
            _colliderListener.OnTriggerExitAction -= TriggerExit;
        }

        private void TriggerEnter(Collider other)
        {
            if (!other.CompareTag(TagMouse)) return;

            PlayerMousePrefabController controller = other.GetComponent<PlayerMousePrefabController>();
            controller.OnBuilding();
            _lightningHolder.SetActive(true);
        }

        private void TriggerExit(Collider other)
        {
            if (!other.CompareTag(TagMouse)) return;

            PlayerMousePrefabController controller = other.GetComponent<PlayerMousePrefabController>();
            controller.ExitBuilding();
            _lightningHolder.SetActive(false);
        }
    }
}
