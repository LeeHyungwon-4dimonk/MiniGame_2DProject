using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellChargeUI : MonoBehaviour
{
    private float m_chargeTime;
    private Image m_image;
    private PlayerController m_playerController;

    private void Awake() => Init();

    private void Init()
    {
        m_image = GetComponent<Image>();
        m_playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        m_image.fillAmount = 0;
    }

    private void Update()
    {
        if (m_playerController.IsCharging == false)
        {
            if (m_image.fillAmount < 0) m_image.fillAmount = 0;
            m_image.fillAmount -= Time.deltaTime;
        }
        else
        {
            m_chargeTime = m_playerController.ChargeTime;
            m_image.fillAmount = m_chargeTime / 6;
        }
    }
}
