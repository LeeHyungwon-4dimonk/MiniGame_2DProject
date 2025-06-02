using UnityEngine;

/// <summary>
/// ���� ���� ������
/// </summary>
public class ManaPotionCreater : PotionCreater
{
    // ������ ȸ����ġ
    private int m_status;

    private void Awake() => Init();

    // ���� ������ 1���� 2 ������ ������ ȸ�� ��ġ�� ������(�뷱��)
    private void Init()
    {
        m_status = Random.Range(1, 3);
    }

    // �÷��̾��� ���� ������ �ִ�ġ�� ������ ���� �Ұ�
    // �÷��̾��� ������ ȸ�������ְ� ��Ȱ��ȭ��
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (GameManager.Instance.GetCurMP() == GameManager.Instance.GetMaxMP()) return;
            Recover();
            gameObject.SetActive(false);
        }
    }

    // �÷��̾��� ������ ȸ����Ŵ
    public override void Recover()
    {
        GameManager.Instance.RecoverMp(m_status);
    }
}