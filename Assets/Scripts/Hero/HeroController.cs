using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
namespace HoangTuan.Scripts.Scriptable_Objects.Character
{
    public class HeroController : CharacterController
    {
        [SerializeField] 
        private float attackRange;
        [SerializeField] 
        private float attackDelay;
        bool canAttack = true;
        public static HeroController me;
        [PunRPC]
        public void InitializeHero(Player player)
        {
            _heroData.heroID = player.ActorNumber;
            photonPlayer = player;
            _heroUI.UpdateInfo(player.NickName, _heroData.heroLevel);
            if (player.IsLocal)
                me = this;
        }
        protected override void Update()
        {
            base.Update();
            if (!photonView.IsMine)
                return;

            MoveCharacter();
            AttackBase();
        }
        void AttackBase()
        {
            if (AttackTarget != null)
            {
                float distanceToTarget = Vector2.Distance(transform.position, AttackTarget.transform.position);
                if (distanceToTarget > attackRange)
                {
                    AttackTarget = null;
                }
            }

            if (Input.GetKeyDown(KeyCode.F) && canAttack)
            {
                CharacterController target = FindClosestEnemy(attackRange);
                if (target != null)
                {
                    SetAttackTarget(target);
                    Attack();
                }
            }
        }
        public override void Attack()
        {
            base.Attack();
            StartCoroutine(AttackCooldown());
            heroAim.SetTrigger("Attack");
        }

        private IEnumerator AttackCooldown()
        {
            canAttack = false;
            yield return new WaitForSeconds(attackDelay);
            canAttack = true;
        }
        #region HERO
        void MoveCharacter()
        {
            if (!dead)
            {
                float x = Input.GetAxis("Horizontal");
                float y = Input.GetAxis("Vertical");

                if (x != 0 || y != 0)
                {
                    rb.velocity = new Vector2(x, y) *_heroData.moveSpeed;
                    heroAim.SetBool("Move", true);

                    if (x > 0 && !faceRight)
                    {
                        photonView.RPC("FlipRight", RpcTarget.All);
                    }
                    else if (x < 0 && faceRight)
                    {
                        photonView.RPC("FlipLeft", RpcTarget.All);
                    }
                }
                else
                {
                    rb.velocity = Vector2.zero;
                    heroAim.SetBool("Move", false);
                }
            }
        }
        [PunRPC]
        void FlipRight()
        {
            faceRight = true;
            Vector3 theScale = _tranformHero.localScale;
            theScale.x = -1;
            _tranformHero.localScale = theScale;
        }
        [PunRPC]
        void FlipLeft()
        {
            faceRight = false;
            Vector3 theScale = _tranformHero.localScale;
            theScale.x = 1;
            _tranformHero.localScale = theScale;
        }
        #endregion
        #region GIZMOS
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(_tranformHero.position, attackRange);
        }
        #endregion
    }
}
