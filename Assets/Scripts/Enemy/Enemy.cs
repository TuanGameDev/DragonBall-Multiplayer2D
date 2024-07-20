using _Game.Scripts.Interfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HoangTuan.Scripts.Scriptable_Objects.Character
{
    public class Enemy : CharacterController, IDamagable
    {
        protected override void Start()
        {
            base.Start();
        }
        public override void TakeDamage(int amount)
        {
            base.TakeDamage(amount);
        }

        public override void Die()
        {
            base.Die();
            // Thêm logic khi Enemy chết, ví dụ: hủy đối tượng
            Destroy(gameObject);
        }
    }
}
