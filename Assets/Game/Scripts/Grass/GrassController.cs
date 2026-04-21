using System;
using UnityEngine;

namespace Game.Scripts.Grass
{
    public class GrassController : MonoBehaviour
    {
        [SerializeField] private LayerMask _layerMask;
        private void Start()
        {
            transform.localScale = Vector3.one;
            gameObject.layer = _layerMask;
        }
    }
}
