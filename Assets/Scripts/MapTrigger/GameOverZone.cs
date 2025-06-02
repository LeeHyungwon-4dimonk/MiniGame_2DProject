using UnityEngine;

public class GameOverZone : MonoBehaviour
{
    /// <summary>
    /// 플레이어와 충돌했을 때 게임오버가 되는 영역에 붙이는 컴포넌트
    /// 플레이어가 바닥 외의 영역 아래로 떨어졌을 때 게임오버 처리를 할 것
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")) GameManager.Instance.GameOver();
    }
}
