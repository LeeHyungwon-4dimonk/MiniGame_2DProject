using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{
    private Image m_image;
    private Text text;
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
