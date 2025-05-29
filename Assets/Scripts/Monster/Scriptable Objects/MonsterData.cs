using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterData : ScriptableObject
{
    public string Name;

    public GameObject Prefab;
    
    public int MonsterHp;
    public int MonsterAtk;
    public int MonsterMoveSpeed;

    public abstract void Attack(PlayerController player);
}
