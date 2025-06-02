using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �ΰ��� �÷��̾� ü�� ��
/// </summary>
public class PlayerMPUI : MonoBehaviour
{
    // ������Ʈ ����
    private Image m_image;
    private Text text;

    // ���� ����
    private int m_maxHp;
    private int m_curHp;

    private void Awake()
    {
        m_image = GetComponentInChildren<Image>();
        text = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        m_maxHp = GameManager.Instance.GetMaxHP();
        m_curHp = GameManager.Instance.GetCurHP();
        text.text = $"{m_curHp} / {m_maxHp}";
        m_image.fillAmount = (float)m_curHp/(float)m_maxHp;
    }

    private void Update()
    {
        m_curHp = GameManager.Instance.GetCurHP();
        m_image.fillAmount = (float)m_curHp/(float)m_maxHp;
        text.text = $"{m_curHp} / {m_maxHp}";
    }
}
