using _Game.Scripts.Interfaces;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
namespace HoangTuan.Scripts.Scriptable_Objects.Character
{
    public class Enemy : CharacterController, IDamagable
    {
        protected override void Start()
        {
            base.Start();
            _heroUI.UpdateInfo(_heroData.heroName, _heroData.heroLevel);
        }
    }
}
