using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �÷��̾��� ���� ���� �ð� �� ��Ÿ����
/// ���������� �����ִ� UI �ý���
/// </summary>
public class SpellChargeUI : MonoBehaviour
{
    // ������Ʈ ����
    private PlayerController m_playerController;
    private Image m_image;

    // ���� �ð� ����
    private float m_chargeTime;

    private void Awake() => Init();

    private void Init()
    {
        m_image = GetComponent<Image>();
        m_playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        m_image.fillAmount = 0;
    }

    private void Update()
    {
        // ��ų ��¡ ���� �ƴϰ� ��Ÿ���� ���� ���� ������
        // ȸ������ ǥ���Ͽ� ��ũ�� ������� ��翡 ���� ��Ÿ�� ����� ���������� ���̰� ��
        // �뷱������ ��ų ��Ÿ�� ���� �ÿ� / 3 �Ǿ� �ִ� �κ��� ������ �ʿ���
        if (m_playerController.IsCharging == false)
        {
            m_image.color = Color.grey;
            m_image.fillAmount = GameManager.Instance.GetCoolTime() / 3;
        }

        // ��ų�� ���� �ð��� ���� �������� �����Ǵ� ���� ���������� ǥ��
        // �ִ� ���� �ð��� 6�� ���ķ� ������ ���ķδ� �̹����� ��ȭ�� ������
        // �߻� �ÿ��� ��Ÿ�� �̹����� ��ȯ��
        // �뷱������ �ִ� ��¡ �ð� ���� �ÿ� / 6 �Ǿ� �ִ� �κ��� ������ �ʿ���
        else
        {
            m_image.color = Color.blue;
            m_chargeTime = m_playerController.ChargeTime;
            m_image.fillAmount = m_chargeTime / 6;
        }
    }
}
