using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HoangTuan.Scripts.Scriptable_Objects.Character
{
    public class CharacterController : MonoBehaviourPun
    {
        public Rigidbody2D rb;
        public Animator heroAim;
        public float moveSpeed;
        public Transform _tranformHero;
        protected virtual void Start()
        {
   
        }
        protected virtual void Update()
        {

        }
    }
}
