using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Game.Scripts.Mobs
{
    public class MobAnimatorController: MonoBehaviour
    {
        private static readonly int State = Animator.StringToHash("State");

        [Header("References")] 
        [SerializeField] private Animator _animator;

        public async UniTask AnimateFood(CancellationToken token)
        {
            Debug.Log($"Start {gameObject.name} Eater Animated");
            
            _animator.SetInteger(State, 1);
            await UniTask.WaitForEndOfFrame(token);
            await UniTask.WaitForSeconds(_animator.GetCurrentAnimatorStateInfo(0).length, cancellationToken: token);
            
            Debug.Log($"End {gameObject.name} Eater Animated");
        }
        
    }
}