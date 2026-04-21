using UnityEngine;

namespace Game.Scripts.Grass
{
    public class GrassController : MonoBehaviour
    {
        public Vector2Int CurrentCell { get; set; }
        
        private void Start()
        {
            transform.localScale = Vector3.one;
        }
    }
}
