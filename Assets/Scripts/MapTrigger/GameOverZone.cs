using UnityEngine;

public class GameOverZone : MonoBehaviour
{
    /// <summary>
    /// �÷��̾�� �浹���� �� ���ӿ����� �Ǵ� ������ ���̴� ������Ʈ
    /// �÷��̾ �ٴ� ���� ���� �Ʒ��� �������� �� ���ӿ��� ó���� �� ��
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")) GameManager.Instance.GameOver();
    }
}
