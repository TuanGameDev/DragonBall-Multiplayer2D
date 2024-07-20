using Photon.Pun;
using UnityEngine;

namespace HoangTuan.Scripts.Scriptable_Objects.Character
{
    public class CharacterAnimations : MonoBehaviourPun
    {
        public Animator _animator;

        private readonly int IDLE = Animator.StringToHash("Idle");
        private readonly int MOVE = Animator.StringToHash("Move");
        private readonly int DIE = Animator.StringToHash("Die");

        private float _transitionDuration = .1f;

        public CharacterAnimations(Animator animator)
        {
            _animator = animator;
        }

        public float GetNormalizedTime()
        {
            var currentInfo = _animator.GetCurrentAnimatorStateInfo(0);
            var nextInfo = _animator.GetNextAnimatorStateInfo(0);

            if (_animator.IsInTransition(0) && nextInfo.IsTag("Attack"))
                return nextInfo.normalizedTime;
            if (!_animator.IsInTransition(0) && currentInfo.IsTag("Attack"))
                return currentInfo.normalizedTime;

            return 0f;
        }

        public void PlayIdle()
        {
            _animator.CrossFadeInFixedTime(IDLE, _transitionDuration);
        }

        public void PlayMove()
        {
            _animator.CrossFadeInFixedTime(MOVE, _transitionDuration);
        }
        public void PlayDead()
        {
            _animator.CrossFadeInFixedTime(DIE, _transitionDuration);
        }

        public void PlayAttack(string attackName)
        {
            _animator.CrossFadeInFixedTime(attackName, _transitionDuration);
        }
    }
}