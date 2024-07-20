using _Game.Scripts.Interfaces;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Animation;
using UnityEngine;

namespace HoangTuan.Scripts.Scriptable_Objects.Character
{
    public class CharacterController : MonoBehaviourPun
    {
        public Rigidbody2D rb;
        public Animator heroAim;
        public Transform _tranformHero;

        [Header("Hero Controller")]
        public HeroSO _heroData;
        public CharacterStat CurrentStat;
        [SerializeField]
        protected StatForAction _maxStat;
        public HeroUI _heroUI;
        public Player photonPlayer;
        public CharacterController AttackTarget;

        [SerializeField]
        private HealthBarUI _healthBarUI;
        public HealthBarUI HealthBarUI => _healthBarUI;
        public Action OnDie;

        [Header("Bool")]
        public bool dead;
        public bool faceRight = false;
        public float lastAttackTime;

        protected virtual void Start()
        {
            CurrentStat.StatForAction.BaseHp = _heroData.BaseStat.StatForAction.BaseHp;
            CurrentStat.StatForAttack.BaseAttack = _heroData.BaseStat.StatForAttack.BaseAttack;
            _maxStat.SetEqual(CurrentStat.StatForAction);

            _healthBarUI.SetHealthBar(CurrentStat.StatForAction.BaseHp, _maxStat.BaseHp, CurrentStat.StatForAction.BaseHp);
        }

        protected virtual void Update()
        {

        }

        public virtual void Attack()
        {
            if (AttackTarget == null) return;
            if (AttackTarget.GetComponent<IDamagable>() == null)
            {
                Debug.Log($"Attack target {AttackTarget.name} not added IDamagable");
                return;
            }
            PerformDamage(AttackTarget.GetComponent<IDamagable>());
        }

        protected virtual void PerformDamage(IDamagable damagable)
        {
            if (damagable == null) return;
            damagable.TakeDamage(CurrentStat.StatForAttack.BaseAttack);
        }


        public virtual void TakeDamage(int amount)
        {
            if (_heroData == null)
            {
                return;
            }

            CurrentStat.StatForAction.BaseHp -= amount;

            if (_healthBarUI != null)
            {
                _healthBarUI.SetHealthBar(CurrentStat.StatForAction.BaseHp, _maxStat.BaseHp, CurrentStat.StatForAction.BaseHp);
            }

            // UIManager.Instance.GenerateFloatingText(Mathf.FloorToInt(amount).ToString(), transform);
            if (CurrentStat.StatForAction.BaseHp <= 0)
            {
                Die();
            }
        }
        protected CharacterController FindClosestEnemy(float range)
        {
            CharacterController closestEnemy = null;
            float closestDistance = range;

            foreach (CharacterController enemy in FindObjectsOfType<CharacterController>())
            {
                if (enemy == this) continue;
                float distance = Vector2.Distance(transform.position, enemy.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy;
                }
            }
            return closestEnemy;
        }
        public void SetAttackTarget(CharacterController target)
        {
            AttackTarget = target;
        }
        public virtual void Die()
        {
            OnDie?.Invoke();
        }
    }
}
