using UnityEngine;

/// <summary>
/// ��� ������ ������ ��� ���� ��ũ���ͺ� ������Ʈ
/// </summary>
[CreateAssetMenu(fileName = "Boss Monster", menuName = "Scriptable Objects/Boss Monster", order = 2)]
public class BossMonsterData : MonsterData
{
    public float SkillCoolTime;

    public override void Attack(PlayerController controller)
    {
        controller.TakeDamage(MonsterAtk);
    }
}
