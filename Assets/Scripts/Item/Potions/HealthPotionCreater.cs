using UnityEngine;

/// <summary>
/// ü�� ���� ������
/// </summary>
public class HealthPotionCreater : PotionCreater
{
    // ������ ȸ�� ��ġ
    private int m_status;

    private void Awake() => Init();

    // ü�� ������ 1���� 4 ������ ������ ȸ�� ��ġ�� ������(�뷱��)
    private void Init()
    {
        m_status = Random.Range(1, 5);
    }

    // �÷��̾��� ���� ü���� �ִ�ġ�� ������ ���� �Ұ�
    // �÷��̾��� ü���� ȸ�������ְ� ��Ȱ��ȭ��
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance.GetCurHP() == GameManager.Instance.GetMaxHP()) return;
            Recover();
            gameObject.SetActive(false);
        }
    }

    // �÷��̾��� ü���� ȸ����Ŵ
    public override void Recover()
    {
        GameManager.Instance.RecoverHp(m_status);
    }
}