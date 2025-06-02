using UnityEngine;

/// <summary>
/// 몬스터 생성을 위한 추상클래스 스크립터블 오브젝트
/// </summary>
public abstract class MonsterData : ScriptableObject
{
    // 몬스터 이름
    public string Name;

    // 몬스터 프리팹
    public GameObject Prefab;
    
    // 몬스터 스텟
    public int MonsterHp;
    public int MonsterAtk;
    public int MonsterMoveSpeed;

    // 몬스터 공격
    public abstract void Attack(PlayerController player);
}