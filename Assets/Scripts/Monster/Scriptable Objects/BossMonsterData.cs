using UnityEngine;

/// <summary>
/// ���� ������ ������ ��� ���� ��ũ���ͺ� ������Ʈ
/// </summary>
[CreateAssetMenu(fileName = "Boss Monster", menuName = "Scriptable Objects/Boss Monster", order = 2)]
public class BossMonsterData : MonsterData
{
    [Header("CoolTime/Delay")]
    // ���� ���� ��Ÿ��
    public float StunApplyCoolTime;
    // ���� ��Ÿ��
    public float StunCoolTime;
    // ���� ���� �� ���ð� ������
    public float RestDelay;
    // ���� �� �ִϸ��̼� ��� ������
    public float DieDelay;

    [Header("Spell Attack")]
    // ��ų ������
    public int SpellAttackDamage;
    // ��ų �ӵ�
    public float SpellSpeed;
    // ��ų ��Ÿ��
    public float SkillCoolTime;

    [Header("Melee Attack")]
    // ���� ���� ���� 
    public float AttackRange;
    // ���� ���� ����(����) ���� - X
    public float AttackRange_X;
    // ���� ���� ����(����) ���� - Y
    public float AttackRange_Y;
    // ���� ���� ������
    public float MeleeAttackDelay;

    public override void Attack(PlayerController controller)
    {
        controller.TakeDamage(MonsterAtk);
    }
}
