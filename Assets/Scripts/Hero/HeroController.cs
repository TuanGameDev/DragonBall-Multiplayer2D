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
        [Header("Hero Controller")]
        public HeroSO _heroData;
        public Player photonPlayer;
        [Header("Bool")]
        public bool dead;
        public bool faceRight = false;
        public static HeroController me;
        [PunRPC]
        public void InitializeHero(Player player)
        {
            _heroData.heroID = player.ActorNumber;
            photonPlayer = player;
            if (player.IsLocal)
                me = this;
        }
        protected override void Update()
        {
            base.Update();
            MoveCharacter();
        }
        void MoveCharacter()
        {
            if (!dead)
            {
                float x = Input.GetAxis("Horizontal");
                float y = Input.GetAxis("Vertical");

                if (x != 0 || y != 0)
                {
                    rb.velocity = new Vector2(x, y) * moveSpeed;
                    heroAim.SetBool("Move", true);

                    if (x > 0 && !faceRight)
                    {
                        FlipRight();
                    }
                    else if (x < 0 && faceRight)
                    {
                        FlipLeft();
                    }
                }
                else
                {
                    rb.velocity = Vector2.zero;
                    heroAim.SetBool("Move", false);
                }
            }
        }
        void FlipRight()
        {
            faceRight = true;
            Vector3 theScale = _tranformHero.localScale;
            theScale.x = -1;
            _tranformHero.localScale = theScale;
        }

        void FlipLeft()
        {
            faceRight = false;
            Vector3 theScale = _tranformHero.localScale;
            theScale.x = 1;
            _tranformHero.localScale = theScale;
        }
    }
}
