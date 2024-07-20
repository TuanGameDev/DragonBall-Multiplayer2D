using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace HoangTuan.Scripts.Scriptable_Objects.Character
{
    [CreateAssetMenu(fileName = ("New"), menuName = ("DataBase/Character"))]
    public class HeroSO : ScriptableObject
    {
        [Header("HeroData")]
        public string heroName;
        public int heroID;
        public int heroLevel;

        [Header("Hero Level")]
        public int currentExp;
        public int maxExp;

        [Header("Hero Level")]
        public int currentHp;
        public int maxHp;

    }
}
