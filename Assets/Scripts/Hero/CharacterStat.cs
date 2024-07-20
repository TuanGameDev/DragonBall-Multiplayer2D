using UnityEngine;
using System;

[Serializable]
public class CharacterStat
{
    [Header("Stat for action")]
    public StatForAction StatForAction = new();

    [Header("Stat for attack")]
    public StatForAttack StatForAttack = new();

    public int ExpToLevelUp;
}

[Serializable]
public class StatForAction
{
    public int BaseHp;

    public void SetEqual(StatForAction stat)
    {
        BaseHp = stat.BaseHp;
    }
}
[Serializable]
public class StatForAttack
{
    public int BaseAttack;
    public int BaseDef;
}