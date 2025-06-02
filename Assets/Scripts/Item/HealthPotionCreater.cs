using UnityEngine;

/// <summary>
/// 체력 포션 생성기
/// </summary>
public class HealthPotionCreater : PotionCreater
{
    // 포션의 회복 수치
    private int m_status;

    private void Awake() => Init();

    // 체력 포션은 1에서 4 사이의 랜덤한 회복 수치로 생성됨(밸런싱)
    private void Init()
    {
        m_status = Random.Range(1, 5);
    }

    // 플레이어의 현재 체력이 최대치일 때에는 습득 불가
    // 플레이어의 체력을 회복시켜주고 비활성화됨
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance.GetCurHP() == GameManager.Instance.GetMaxHP()) return;
            Recover();
            gameObject.SetActive(false);
        }
    }

    // 플레이어의 체력을 회복시킴
    public override void Recover()
    {
        GameManager.Instance.RecoverHp(m_status);
    }
}