using UnityEngine;

public class PortalTrigger : MonoBehaviour
{
    [SerializeField] private SceneName m_sceneName;

    /// <summary>
    /// �÷��̾�� �浹���� �� �� ��ȯ���ִ� Ʈ����
    /// �ν����Ϳ��� Serializable�� �̵��� �� �ִ� ���� ������� �����Ͽ� ���
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