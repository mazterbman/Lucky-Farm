using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Scripts.Grass
{
    [Serializable]
    public class GrassData
    {
        public GrassManager Manager;
        public Transform Parent;
        public AssetReferenceGameObject GrassPrefab;
    }
}