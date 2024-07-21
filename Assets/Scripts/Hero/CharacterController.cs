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
        public int curAttackerID;
        protected virtual void Start()
        {
            CurrentStat.StatForAction.BaseHp = _heroData.BaseStat.StatForAction.BaseHp;
            CurrentStat.StatForAttack.BaseAttack = _heroData.BaseStat.StatForAttack.BaseAttack;
            _maxStat.SetEqual(CurrentStat.StatForAction);
        }

        protected virtual void Update()
        {
            UpdateHealthUI(CurrentStat.StatForAction.BaseHp);
        }
        [PunRPC]
        public virtual void Attack()
        {
            if (AttackTarget == null) return;
            if (AttackTarget.GetComponent<IDamagable>() == null)
            {
                return;
            }
            heroAim.SetTrigger("Attack");
            photonView.RPC("PerformDamage", RpcTarget.All, AttackTarget._heroData.heroID, CurrentStat.StatForAttack.BaseAttack, _heroData.heroID);
        }

        [PunRPC]
        protected virtual void PerformDamage(int targetHeroID, int damageAmount, int attackerID)
        {
            CharacterController target = FindCharacterByHeroID(targetHeroID);
            if (target != null && target is IDamagable damagable)
            {
                damagable.TakeDamage(damageAmount, attackerID);
            }
        }

        private CharacterController FindCharacterByHeroID(int heroID)
        {
            foreach (var character in FindObjectsOfType<CharacterController>())
            {
                if (character._heroData.heroID == heroID)
                {
                    return character;
                }
            }
            return null;
        }
        [PunRPC]
        public virtual void TakeDamage(int amount, int attackerID)
        {
            if (_heroData == null)
            {
                return;
            }
            CurrentStat.StatForAction.BaseHp -= amount;
            curAttackerID = attackerID;
            if (_healthBarUI != null)
            {
                _healthBarUI.SetHealthBar(CurrentStat.StatForAction.BaseHp, _maxStat.BaseHp, CurrentStat.StatForAction.BaseHp);
            }

            if (CurrentStat.StatForAction.BaseHp <= 0)
            {
                Die();
            }
        }

        void UpdateHealthUI(int currentHp)
        {
            CurrentStat.StatForAction.BaseHp = currentHp;
            if (_healthBarUI != null)
            {
                _healthBarUI.SetHealthBar(CurrentStat.StatForAction.BaseHp, _maxStat.BaseHp, CurrentStat.StatForAction.BaseHp);
            }
        }
        public void SetAttackTarget(CharacterController target)
        {
            AttackTarget = target;
        }
        public virtual void Die()
        {
            heroAim.SetTrigger("Die");
            OnDie?.Invoke();
        }
    }
}
