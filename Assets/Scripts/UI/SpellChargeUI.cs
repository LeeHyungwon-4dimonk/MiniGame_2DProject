using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 스펠 차지 시간 및 쿨타임을
/// 가시적으로 보여주는 UI 시스템
/// </summary>
public class SpellChargeUI : MonoBehaviour
{
    // 컴포넌트 참조
    private PlayerController m_playerController;
    private Image m_image;

    // 차지 시간 저장
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
        // 스킬 차징 중이 아니고 쿨타임이 돌고 있을 때에는
        // 회색으로 표시하여 마크가 사라지는 모양에 따라 쿨타임 경과가 가시적으로 보이게 함
        // 밸런싱으로 스킬 쿨타임 조정 시에 / 3 되어 있는 부분의 조정이 필요함
        if (m_playerController.IsCharging == false)
        {
            m_image.color = Color.grey;
            m_image.fillAmount = GameManager.Instance.GetCoolTime() / 3;
        }

        // 스킬의 차지 시간에 따라 게이지가 충전되는 것을 가시적으로 표현
        // 최대 충전 시간인 6초 이후로 충전한 이후로는 이미지에 변화가 없으며
        // 발사 시에는 쿨타임 이미지로 전환됨
        // 밸런싱으로 최대 차징 시간 조정 시에 / 6 되어 있는 부분의 조정이 필요함
        else
        {
            m_image.color = Color.blue;
            m_chargeTime = m_playerController.ChargeTime;
            m_image.fillAmount = m_chargeTime / 6;
        }
    }
}
