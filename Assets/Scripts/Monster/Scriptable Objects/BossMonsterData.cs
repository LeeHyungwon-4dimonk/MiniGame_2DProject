using UnityEngine;

/// <summary>
/// 보스 몬스터의 정보를 담기 위한 스크립터블 오브젝트
/// </summary>
[CreateAssetMenu(fileName = "Boss Monster", menuName = "Scriptable Objects/Boss Monster", order = 2)]
public class BossMonsterData : MonsterData
{
    [Header("CoolTime/Delay")]
    // 스턴 적용 쿨타임
    public float StunApplyCoolTime;
    // 스턴 쿨타임
    public float StunCoolTime;
    // 근접 공격 후 대기시간 딜레이
    public float RestDelay;
    // 죽은 후 애니메이션 출력 딜레이
    public float DieDelay;

    [Header("Spell Attack")]
    // 스킬 데미지
    public int SpellAttackDamage;
    // 스킬 속도
    public float SpellSpeed;
    // 스킬 쿨타임
    public float SkillCoolTime;

    [Header("Melee Attack")]
    // 근접 공격 범위 
    public float AttackRange;
    // 근접 공격 시작(판정) 범위 - X
    public float AttackRange_X;
    // 근접 공격 시작(판정) 범위 - Y
    public float AttackRange_Y;
    // 근접 공격 딜레이
    public float MeleeAttackDelay;

    public override void Attack(PlayerController controller)
    {
        controller.TakeDamage(MonsterAtk);
    }
}
