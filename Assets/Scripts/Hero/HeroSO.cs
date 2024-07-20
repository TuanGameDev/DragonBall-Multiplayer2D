using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HoangTuan.Scripts.Scriptable_Objects.Character
{
    [CreateAssetMenu(fileName = ("New"), menuName = ("DataBase/Character"))]
    public class HeroSO : ScriptableObject
    {
        [Header("HeroData")]
        public int heroID;
        public int heroLevel;
        public float moveSpeed;

        public CharacterStat BaseStat = new();
    }
}
