using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using Zenject;

namespace Game.Scripts.Mobs
{
    public class MobPathController : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private NavMeshAgent _navMeshAgent = null;
        
        [Header("References Injected")] [Inject] [SerializeField]
        private NavMeshSurface _meshSurface = null;

        private void Start()
        {
            
        }
    }
}
