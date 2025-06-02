using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    [SerializeField] private SceneName m_sceneName;

    /// <summary>
    /// 플레이어와 충돌했을 때 씬 전환해주는 트리거
    /// 인스펙터에서 Serializable로 이동할 수 있는 씬을 목록으로 선택하여 사용
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneChanger.Instance.SceneChange(m_sceneName);
        }
    }
}