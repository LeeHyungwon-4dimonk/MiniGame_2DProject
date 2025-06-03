using UnityEngine;

/// <summary>
/// 노멀 몬스터의 정보를 담기 위한 스크립터블 오브젝트
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
