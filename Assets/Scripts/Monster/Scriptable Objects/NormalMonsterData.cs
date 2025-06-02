using UnityEngine;

/// <summary>
/// 노멀 몬스터의 정보를 담기 위한 스크립터블 오브젝트
/// </summary>
[CreateAssetMenu(fileName = "Normal Monster", menuName = "Scriptable Objects/Normal Monster", order = 1)]
public class NormalMonsterData : MonsterData
{
    // 몬스터 탐지 시야
    public float MonsterSight;
    // 일반 몬스터는 잡을 시에 점수를 줌
    public int Score;

    // 몬스터 공격
    public override void Attack(PlayerController controller)
    {
        controller.TakeDamage(MonsterAtk);
    }
}