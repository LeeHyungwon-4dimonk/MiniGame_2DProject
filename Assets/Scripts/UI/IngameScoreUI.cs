using TMPro;
using UnityEngine;

/// <summary>
/// 인게임의 점수를 실시간으로 표기하는 UI
/// </summary>
public class IngameScoreUI : MonoBehaviour
{
    private int score;
    [SerializeField] TMP_Text text;
    private void Update()
    {
        score = GameManager.Instance.GetScore();
        text.text = $"점수 : {score.ToString("000")}";
    }
}
