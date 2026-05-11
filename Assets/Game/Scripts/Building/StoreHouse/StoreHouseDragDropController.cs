using System;
using Game.Scripts.Player;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;

namespace Game.Scripts.Building.StoreHouse
{
    [RequireComponent(typeof(RectTransform))]
    public class StoreHouseDragDropController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private EventTrigger _eventTrigger;

        [Inject] private BuildingData _buildingData;
        [Inject] private PlayerData _playerData;
        
        private EventTrigger.Entry _dragEntry;
        private EventTrigger.Entry _beginDragEntry;
        private EventTrigger.Entry _dropEntry;
        private Transform _oldTransformParent;
        private RectTransform _rectTransform;
        
        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            InitializeEventTrigger();
        }

        private void OnDestroy()
        {
            UnInitializeEventTrigger();
        }

        private void InitializeEventTrigger()
        {
            EventTrigger.Entry dragEntry = _eventTrigger.triggers.Find(t => t.eventID == EventTriggerType.Drag) ?? new EventTrigger.Entry
            {
                eventID = EventTriggerType.Drag
            };
            dragEntry.callback.AddListener(Drag);
            _dragEntry = dragEntry;
            
            EventTrigger.Entry beginDragEntry = _eventTrigger.triggers.Find(t => t.eventID == EventTriggerType.BeginDrag) ?? new EventTrigger.Entry
            {
                eventID = EventTriggerType.BeginDrag
            };
            beginDragEntry.callback.AddListener(BeginDrag);
            _beginDragEntry = beginDragEntry;
            
            EventTrigger.Entry dropEntry = _eventTrigger.triggers.Find(t => t.eventID == EventTriggerType.Drop) ?? new EventTrigger.Entry
            {
                eventID = EventTriggerType.Drop
            };
            dropEntry.callback.AddListener(Drop);
            _dropEntry = dropEntry;
        }

        private void UnInitializeEventTrigger()
        {
            _dropEntry?.callback.RemoveListener(Drop);
            _dragEntry?.callback.RemoveListener(Drag);
            _beginDragEntry?.callback.RemoveListener(BeginDrag);
        }

        private void Drop(BaseEventData arg0)
        {
            Debug.Log($"Drop! {gameObject.name}");
        }

        private void Drag(BaseEventData arg0)
        {
            Canvas canvas = _buildingData.StoreHouseController.Canvas;
            Vector2 position =  ScreenToCanvasPosition(canvas, Mouse.current.position.value);
            _rectTransform.localPosition = position;
        }

        private void BeginDrag(BaseEventData arg0)
        {
            Debug.Log($"Begin Drag! {gameObject.name}");
            _oldTransformParent = transform.parent;
            transform.SetParent(_buildingData.StoreHouseController.ParentForDrag);
        }
        
        private static Vector3 ScreenToCanvasPosition(Canvas canvas, Vector3 screenPosition)
        {
            var viewportPosition = new Vector3(screenPosition.x / Screen.width,
                screenPosition.y / Screen.height,
                0);
            return ViewportToCanvasPosition(canvas, viewportPosition);
        }
        
        private static Vector3 ViewportToCanvasPosition(Canvas canvas, Vector3 viewportPosition)
        {
            var centerBasedViewPortPosition = viewportPosition - new Vector3(0.5f, 0.5f, 0);
            var canvasRect = canvas.GetComponent<RectTransform>();
            var scale = canvasRect.sizeDelta;
            return Vector3.Scale(centerBasedViewPortPosition, scale);
        }
    }
}