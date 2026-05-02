using System;
using Unity.AI.Navigation;
using UnityEngine;

namespace Game.Scripts.Mobs
{
    [Serializable]
    public class MobData
    {
        public NavMeshSurface MeshSurface = null;
        public MobAnimalSpawner MobAnimalSpawner;
        public MobAnimalManager MobAnimalManager;
        public Transform ParentItems;
        public Transform ParentMobs;
    }
}