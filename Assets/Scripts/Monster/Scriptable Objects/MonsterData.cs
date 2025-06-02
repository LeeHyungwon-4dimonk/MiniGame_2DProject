using UnityEngine;

/// <summary>
/// ���� ������ ���� �߻�Ŭ���� ��ũ���ͺ� ������Ʈ
/// </summary>
public abstract class MonsterData : ScriptableObject
{
    // ���� �̸�
    public string Name;

    // ���� ������
    public GameObject Prefab;
    
    // ���� ����
    public int MonsterHp;
    public int MonsterAtk;
    public int MonsterMoveSpeed;

    // ���� ����
    public abstract void Attack(PlayerController player);
}