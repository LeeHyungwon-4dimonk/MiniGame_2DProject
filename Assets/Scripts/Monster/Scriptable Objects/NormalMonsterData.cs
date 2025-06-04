using UnityEngine;

/// <summary>
/// 노멀 몬스터의 정보를 담기 위한 스크립터블 오브젝트
/// </summary>
[CreateAssetMenu(fileName = "Normal Monster", menuName = "Scriptable Objects/Normal Monster", order = 1)]
public class NormalMonsterData : MonsterData
{
    [Header("CoolTime/Delay")]
    // 스턴 쿨타임
    public float StunCoolTime;
    // 방향 전환 후 대기시간
    public float RestDelay;
    // 죽은 후 애니메이션 출력 딜레이
    public float DieDelay;

    [Header("Detect")]
    // 몬스터 탐지 시야
    public float MonsterSight;

    [Header("MeleeAttack")]
    // 근접 공격 범위 
    public float AttackRange;
    // 근접 공격 시작(판정) 범위 - X
    public float AttackRange_X;
    // 근접 공격 시작(판정) 범위 - Y
    public float AttackRange_Y;
    // 근접 공격 딜레이
    public float MeleeAttackDelay;

    // 몬스터 공격
    public override void Attack(PlayerController controller)
    {
        controller.TakeDamage(MonsterAtk);
    }
}