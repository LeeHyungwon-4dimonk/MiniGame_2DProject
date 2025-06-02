using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 인게임 플레이어 마나 바
/// </summary>
public class PlayerHPUI : MonoBehaviour
{
    // 컴포넌트 참조
    private Image m_image;
    private Text text;

    // 변수 저장
    private int m_maxMp;
    private int m_curMp;

    private void Awake()
    {
        m_image = GetComponentInChildren<Image>();
        text = GetComponentInChildren<Text>();
    }

    private void Start()
    {
        m_maxMp = GameManager.Instance.GetMaxMP();
        m_curMp = GameManager.Instance.GetCurMP();
        text.text = $"{m_curMp} / {m_maxMp}";
        m_image.fillAmount = (float)m_curMp/(float)m_maxMp;
    }

    private void Update()
    {
        m_curMp = GameManager.Instance.GetCurMP();
        m_image.fillAmount = (float)m_curMp/(float)m_maxMp;
        text.text = $"{m_curMp} / {m_maxMp}";
    }
}
