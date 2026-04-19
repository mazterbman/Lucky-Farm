using System;
using UnityEngine;
using Zenject;

namespace Game.Scripts.Mobs
{
    public class TrackPlayerView : MonoBehaviour
    {
        [Header("References Injected")] [Inject] [SerializeField]
        private Camera _camera = null;

        private void Update()
        {
            transform.LookAt(_camera.transform);
        }
    }
}
