using UnityEngine;

/// <summary>
/// ��� ������ ������ ��� ���� ��ũ���ͺ� ������Ʈ
/// </summary>
[CreateAssetMenu(fileName = "Normal Monster", menuName = "Scriptable Objects/Normal Monster", order = 1)]
public class NormalMonsterData : MonsterData
{
    [Header("CoolTime/Delay")]
    // ���� ��Ÿ��
    public float StunCoolTime;
    // ���� ��ȯ �� ���ð�
    public float RestDelay;
    // ���� �� �ִϸ��̼� ��� ������
    public float DieDelay;

    [Header("Detect")]
    // ���� Ž�� �þ�
    public float MonsterSight;

    [Header("MeleeAttack")]
    // ���� ���� ���� 
    public float AttackRange;
    // ���� ���� ����(����) ���� - X
    public float AttackRange_X;
    // ���� ���� ����(����) ���� - Y
    public float AttackRange_Y;
    // ���� ���� ������
    public float MeleeAttackDelay;

    // ���� ����
    public override void Attack(PlayerController controller)
    {
        controller.TakeDamage(MonsterAtk);
    }
}